import { useState, useEffect } from 'react';
import Form from 'UI/Form';
import Fieldset from 'UI/SimpleForm/Fieldset';
import Select from 'UI/SimpleForm/Select';
import Input from 'UI/SimpleForm/Input';
import Button from 'UI/Button';

import TabSet from 'UI/TabSet';
import Tab from 'UI/TabSet/Tab';

export default function TabSetTest(props) {
	//const { } = props;
	//const [variant, setVariant] = useState();
	//const [outlined, setOutlined] = useState(false);
	//const [rounded, setRounded] = useState(false);
	//const [disabled, setDisabled] = useState(false);
	//const [closeProps, setCloseProps] = useState({});
	const [links, setLinks] = useState(false);
	const [selectedIndex, setSelectedIndex] = useState(1);

	/*
	useEffect(() => {
		setCloseProps({
			variant: variant,
			outline: outlined || undefined,
			rounded: rounded || undefined,
			disabled: disabled || undefined,
		});
	}, [variant, outlined, rounded, disabled]);
	*/

	return <div className="tabset-test">

		<Form sm className="tabset-test__options">
			<Fieldset>
				{/*
				<Select label={`Variant`} value={variant} onChange={(e) => setVariant(e.target.value)}>
					<option value="">None</option>
					<option value="primary">Primary</option>
					<option value="secondary">Secondary</option>
					<option value="success">Success</option>
					<option value="danger">Danger</option>
					<option value="warning">Warning</option>
					<option value="info">Info</option>
				</Select>
				<Select label={`Size`} value={size} onChange={(e) => setSize(e.target.value)}>
					<option value="xs">Extra small</option>
					<option value="sm">Small</option>
					<option value="md">Medium</option>
					<option value="lg">Large</option>
					<option value="xl">Extra large</option>
				</Select>
				<Input type="checkbox" label={`Outlined`} checked={outlined} onChange={() => setOutlined(!outlined)} />
				<Input type="checkbox" label={`Rounded`} checked={rounded} onChange={() => setRounded(!rounded)} />
				<Input type="checkbox" label={`Disabled`} checked={disabled} onChange={() => setDisabled(!disabled)} />
		*/}
				<Input type="checkbox" label={`Show as links`} checked={links} onChange={() => setLinks(!links)} />
			</Fieldset>
		</Form>

		{/*<h2 className="component-test-category">Close buttons</h2>*/}
		<TabSet name="months" selectedIndex={selectedIndex} links={links ? true : undefined} onChange={(e) => setSelectedIndex(e)}>
			<Tab label="Jan" hash="jan">
				<h2>
					January content
				</h2>
				<p>Lorem ipsum dolor sit amet consectetur, adipisicing elit. Maiores iure praesentium sapiente aliquid tempora corporis laborum numquam sunt repellendus possimus beatae odit illum officia dicta enim maxime saepe harum esse blanditiis tenetur ea, rem at voluptatem itaque? Culpa ea quos excepturi voluptas. Commodi necessitatibus ratione, id, maiores aperiam expedita suscipit repellendus illum eligendi saepe, ex veniam a eaque minus aliquam? Corrupti veritatis, optio voluptas ullam consequatur vel explicabo quasi recusandae distinctio quas accusantium nihil libero repellat esse consequuntur beatae quis quaerat perspiciatis neque odio sit, totam ipsa minima. Exercitationem sint sapiente dolorum possimus consequatur explicabo quo quis itaque tempora fugit voluptas, facere perferendis voluptates deleniti, maiores aliquam eum hic natus cupiditate, expedita a? Esse voluptas hic debitis maxime ipsum veritatis amet vero sit, ab rem quidem beatae, commodi voluptatum! In, quam! Nisi ad, nihil explicabo ducimus saepe voluptatum hic nam est veritatis aspernatur aliquam doloremque corporis quos fuga illum ratione, delectus veniam? Minus, aperiam! Doloribus harum animi ullam illum deleniti beatae nemo optio velit doloremque pariatur repellat eos dolor ratione nulla totam similique, vitae corrupti! Sequi, beatae, tenetur saepe expedita blanditiis soluta quod praesentium officia libero consectetur, placeat corrupti culpa fuga neque. Molestiae ratione fugit nam quia dolorum ipsam itaque totam. Doloribus repudiandae ratione atque provident! Eaque ipsum, autem quod maiores dolores velit modi illum incidunt veniam! Consectetur, quo ad et neque quisquam minima vero non, ducimus quae illum necessitatibus ea maiores in totam, illo molestias nostrum eveniet nulla. Eveniet, maiores non molestias, eaque cupiditate maxime ut porro sint ab ipsa labore distinctio nam sed accusamus fugiat autem vero incidunt debitis dolor. Quas, iusto asperiores dignissimos neque aut officia blanditiis incidunt? Aliquam repellat corrupti doloribus quasi! Natus, ipsum adipisci? Pariatur, temporibus tempora! Corporis autem officiis fuga deleniti delectus minus aspernatur inventore, recusandae facere facilis minima, sed odio, incidunt itaque laborum.</p>
			</Tab>
			<Tab label="Feb" hash="feb">
				<h2>
					February content
				</h2>
				<p>
					Lorem ipsum dolor sit amet consectetur adipisicing elit. Odit consequuntur reprehenderit in amet, eligendi aut voluptatibus officia sit distinctio, exercitationem, quam sapiente modi sed! Doloremque tenetur vel, vitae tempore quae molestiae quam, autem, libero nulla magni quisquam. Facere deleniti magnam cumque vitae quod porro iure voluptatem ducimus error repellat nisi eveniet hic itaque eos provident asperiores, delectus nemo quibusdam voluptatum quos autem quidem. Soluta laborum eos quaerat laboriosam possimus? Libero accusantium omnis, distinctio perferendis reiciendis atque laboriosam eveniet sunt magnam, temporibus id iure non similique architecto maxime officiis, ad eius totam dolore eaque? Eveniet incidunt aliquid nihil mollitia! Animi provident fuga aperiam consequuntur commodi amet doloribus inventore? Iste et officia odit labore quam, mollitia hic facilis a autem aut cupiditate eaque dolores veniam impedit maxime ad, harum, dolorum qui recusandae omnis dolorem non. Laborum tempore recusandae quibusdam, exercitationem fuga odio possimus mollitia qui distinctio eaque eius ea voluptatum doloremque a ipsam, dignissimos facere eos repellendus perferendis fugit officiis? Iste quis suscipit esse veniam ex. Iusto eveniet ad dignissimos, perferendis unde cumque qui voluptatem iure molestiae mollitia quaerat vitae itaque temporibus incidunt cupiditate sunt, neque sapiente repudiandae? Vel quaerat consectetur repellendus est repudiandae! Ratione repudiandae quod alias tempore ducimus corrupti. Officiis!
				</p>
			</Tab>
			<Tab label="Mar" hash="mar">
				<h2>
					March content
				</h2>
				<p>
					Lorem ipsum dolor sit amet consectetur, adipisicing elit. Labore quos expedita ea non pariatur, molestiae voluptatibus? Quam facere porro nostrum nobis excepturi delectus quae magni. Quae consequatur, debitis impedit aliquam architecto praesentium officia porro beatae nihil dolorum quia! Eligendi, alias quasi voluptate exercitationem asperiores porro praesentium iusto accusantium voluptates ducimus? Optio illum, corporis hic quo placeat odio quae consequuntur reprehenderit amet, quibusdam expedita architecto quod, eaque rem aut corrupti deserunt delectus veritatis voluptates tenetur cumque. Eius ipsum alias quia itaque debitis iure quam ut aspernatur, harum pariatur necessitatibus eum? Incidunt omnis sit ratione temporibus possimus obcaecati accusantium iste nostrum impedit odit, pariatur saepe dignissimos, deserunt dolor velit laboriosam eveniet animi mollitia exercitationem maiores non. Culpa perferendis veniam omnis at, aliquid odio mollitia consequatur incidunt sequi cumque qui ullam illo dolorem distinctio architecto esse ut? Nisi, eius! Repellendus quas recusandae totam, in a dolores eaque iusto, non facere ipsum quibusdam natus consequuntur, maiores voluptatibus voluptates. Dolor sint sapiente adipisci. Quas possimus consequatur ut quod debitis voluptatum adipisci vitae recusandae illum, odit corporis aliquid illo ex beatae natus expedita accusantium error dolorum, autem nobis. Voluptates ea dignissimos totam! Voluptates quibusdam inventore doloribus quos, mollitia, fuga necessitatibus ipsam temporibus eveniet sint blanditiis! Eveniet illum dolores est sed delectus. Libero ut id, consectetur deserunt sapiente est mollitia quod cupiditate harum numquam inventore aliquid repellat cumque facere labore molestias, rerum ad necessitatibus? Fugit, quo omnis?
				</p>
			</Tab>
			<Tab label="Excessively-long label" hash="long-label-test">
				<h2>
					Last tab content
				</h2>
				<p>
					Artisan bitters seitan vinyl vice wolf chartreuse street art lo-fi. Bruh bespoke grailed, fit semiotics cred neutral milk hotel taxidermy hell of gastropub hoodie kombucha try-hard la croix. Locavore narwhal woke mumblecore, photo booth vegan chia. Biodiesel tilde vinyl jawn hell of. Cronut hexagon letterpress post-ironic kitsch, health goth slow-carb.
				</p>
				<p>
					VHS coloring book meh aesthetic schlitz intelligentsia hexagon try-hard pabst. Jean shorts ugh mixtape, 90's master cleanse lyft typewriter. Tacos letterpress quinoa pug iceland vegan vinyl stumptown beard taxidermy tonx portland keffiyeh heirloom. Bicycle rights heirloom iceland, mukbang farm-to-table polaroid fixie gorpcore wayfarers ennui chicharrones irony. Prism pok pok squid marfa, snackwave lomo chicharrones. Actually selvage narwhal lo-fi 8-bit pickled, vegan schlitz freegan austin letterpress.
				</p>
			</Tab>
		</TabSet>

		<small>
			Selected index: {selectedIndex}
		</small>
	</div>;
}

TabSetTest.propTypes = {
};

TabSetTest.defaultProps = {
}

TabSetTest.icon='align-center';
