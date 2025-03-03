import { getType } from './Types';

export default class GraphNode {
	constructor(props){
		this.state = {};
		this.props = props;
		this.offsetX = props.offsetX;
		this.offsetY = props.offsetY;
	}
	
	getModulePath(){
		if(!this.constructor.__modulePath){
			
			// Find the module path:
			for(var path in __mm){
				var module = __mm[path];
				if(module && module.v && module.v.default == this.constructor){
					
					// Found it!
					this.constructor.__modulePath = path;
					break;
				}
			}
			
		}
		
		return this.constructor.__modulePath;
	}
	
	toJson(){
		var links = undefined;
		var data = undefined;
		
		if(Array.isArray(this.state)){
			
			this.state.forEach((value, key) => {
				
				if(value && value.link && value.node){
					
					if(!links){
						links = {};
					}
					
					// It's a link.
					links[key] = {n: value.node.arrayIndex, f: value.field};
					
				}else{
					
					if(!data){
						data = {};
					}
					
					// Data.
					data[key] = value;
					
				}
				
			});
			
		}else{
			
			for(var key in this.state){
				
				var value = this.state[key];
				
				if(value && value.link && value.node){
					// It's a link.
					if(!links){
						links = {};
					}
					
					links[key] = {n: value.node.arrayIndex, f: value.field};
				}else{
					// Data.
					if(!data){
						data = {};
					}
					
					data[key] = value;
				}
				
			}
			
		}
		
		var modulePath = this.getModulePath();
		
		var result = {
			t: modulePath,
			d: data,
			l: links,
			x: this.offsetX,
			y: this.offsetY,
			r: this.root
		};
		
		this.onOutputJson && this.onOutputJson(result);
		
		return result;
	}
	
	validateState(){
		// Do any custom manipulation to load internal state based on the fully loaded this.state
		return Promise.resolve(true);
	}
	
	setType(mainOutputType){
		var typeInfo = getType(mainOutputType);
		this._typeColor = typeInfo.color;
	}
	
	connect(field, toNode, toNodeField){
		
		// Note that this.state can be an array too.
		this.state[field.key] = {link: true, node: toNode, field: toNodeField.key};
		
	}
	
	hasConnectionTo(node, fieldKey){
		for(var key in this.state){
			var value = this.state[key];
			
			if(value && value.link && value.node == node && value.field == fieldKey){
				// Yep!
				return true;
			}
		}
		
		// Nope!
		return false;
	}
	
	isOutputConnected(fieldKey){
		// Find a node in the graph with link:true, node: this, field: fieldKey in its state.
		if(!this.graph){
			throw new Error("Graph node requires knowing which graph it is in to be able to establish if an output is in use or not.");
		}
		
		var nodes = this.graph.nodes;
		for(var k in nodes){
			var node = nodes[k];
			
			if(node && node.hasConnectionTo(this, fieldKey)){
				return true;
			}
		}
		
		return false;
	}
	
	resolveValue(link){
		if(!link){
			return null;
		}
		
		if(link.node && link.field !== undefined){
			// It's a link - return the upstream value, assuming it will be a constant node.
			
			if(link.node.state){
				return link.node.state[link.field];
			}
			
			return null;
		}
		
		// as-is.
		return link;
	}
	
	/*
	Gets the type of an input field 
	*/
	getInputType(link){
		if(!link || !link.node || link.field === undefined){
			return;
		}
		
		return link.node.getFieldType(link.field);
	}
	
	getField(fieldKey){
		if(!this.fields){
			this.fields = this.renderFields();
		}
		
		if(!this.fields){
			return;
		}
		
		var field = this.fields.find(field => field.key == fieldKey);
		return field;
	}
	
	getFieldType(fieldKey){
		var field = this.getField(fieldKey);
		return field ? field.type : undefined;
	}
	
	update(context){
		this.context = context;
		this.fields = this.renderFields();
		return this.fields;
	}
	
	getName(){
		return this.constructor.name;
	}
	
	getTypeColor(){
		return this._typeColor || [0.525, 0.42, 0.73];
	}
	
	renderFields(){
		return {};
	}
}
