const digitCharacters = [
	"0",
	"1",
	"2",
	"3",
	"4",
	"5",
	"6",
	"7",
	"8",
	"9",
	"A",
	"B",
	"C",
	"D",
	"E",
	"F",
	"G",
	"H",
	"I",
	"J",
	"K",
	"L",
	"M",
	"N",
	"O",
	"P",
	"Q",
	"R",
	"S",
	"T",
	"U",
	"V",
	"W",
	"X",
	"Y",
	"Z",
	"a",
	"b",
	"c",
	"d",
	"e",
	"f",
	"g",
	"h",
	"i",
	"j",
	"k",
	"l",
	"m",
	"n",
	"o",
	"p",
	"q",
	"r",
	"s",
	"t",
	"u",
	"v",
	"w",
	"x",
	"y",
	"z",
	"#",
	"$",
	"%",
	"*",
	"+",
	",",
	"-",
	".",
	":",
	";",
	"=",
	"?",
	"@",
	"[",
	"]",
	"^",
	"_",
	"{",
	"|",
	"}",
	"~"
];

const decode83 = str => {
	let value = 0;
	for (let i = 0; i < str.length; i++) {
		const c = str[i];
		const digit = digitCharacters.indexOf(c);
		value = value * 83 + digit;
	}
	return value;
};

const sRGBToLinear = value => {
	let v = value / 255;
	if (v <= 0.04045) {
		return v / 12.92;
	} else {
		return Math.pow((v + 0.055) / 1.055, 2.4);
	}
};

const linearTosRGB = value => {
	let v = Math.max(0, Math.min(1, value));
	if (v <= 0.0031308) {
		return Math.round(v * 12.92 * 255 + 0.5);
	} else {
		return Math.round(
			(1.055 * Math.pow(v, 1 / 2.4) - 0.055) * 255 + 0.5
		);
	}
};

const sign = n => (n < 0 ? -1 : 1);

const signPow = (val, exp) => sign(val) * Math.pow(Math.abs(val), exp);

const decodeDC = value => {
	const intR = value >> 16;
	const intG = (value >> 8) & 255;
	const intB = value & 255;
	return [sRGBToLinear(intR), sRGBToLinear(intG), sRGBToLinear(intB)];
};

const decodeAC = (value, maximumValue) => {
	const quantR = Math.floor(value / (19 * 19));
	const quantG = Math.floor(value / 19) % 19;
	const quantB = value % 19;

	const rgb = [
		signPow((quantR - 9) / 9, 2.0) * maximumValue,
		signPow((quantG - 9) / 9, 2.0) * maximumValue,
		signPow((quantB - 9) / 9, 2.0) * maximumValue
	];

	return rgb;
};

var decode = (blurhash, width, height, punch = 1.0) => {

	punch = punch | 1;

	const sizeFlag = decode83(blurhash[0]);
	const numY = Math.floor(sizeFlag / 9) + 1;
	const numX = (sizeFlag % 9) + 1;

	const quantisedMaximumValue = decode83(blurhash[1]);
	const maximumValue = (quantisedMaximumValue + 1) / 166;

	const colors = new Array(numX * numY);

	for (let i = 0; i < colors.length; i++) {
		if (i === 0) {
			const value = decode83(blurhash.substring(2, 6));
			colors[i] = decodeDC(value);
		} else {
			const value = decode83(
				blurhash.substring(4 + i * 2, 6 + i * 2)
			);
			colors[i] = decodeAC(value, maximumValue * punch);
		}
	}

	const bytesPerRow = width * 4;
	const pixels = new Uint8ClampedArray(bytesPerRow * height);

	for (var y = 0; y < height; y++) {
		for (var x = 0; x < width; x++) {
			var r = 0;
			var g = 0;
			var b = 0;

			for (var j = 0; j < numY; j++) {
				for (var i = 0; i < numX; i++) {
					const basis =
						Math.cos((Math.PI * x * i) / width) *
						Math.cos((Math.PI * y * j) / height);
					var color = colors[i + j * numX];
					r += color[0] * basis;
					g += color[1] * basis;
					b += color[2] * basis;
				}
			}

			var intR = linearTosRGB(r);
			var intG = linearTosRGB(g);
			var intB = linearTosRGB(b);

			pixels[4 * x + 0 + y * bytesPerRow] = intR;
			pixels[4 * x + 1 + y * bytesPerRow] = intG;
			pixels[4 * x + 2 + y * bytesPerRow] = intB;
			pixels[4 * x + 3 + y * bytesPerRow] = 255; // alpha
		}
	}
	return pixels;
};

var drawImageDataOnNewCanvas = (imgData, width, height) => {
	const canvas = document.createElement("canvas");
	const ctx = canvas.getContext("2d");
	canvas.width = width;
	canvas.height = height;
	ctx.width = width;
	ctx.height = height;
	ctx.putImageData(new ImageData(imgData, width, height), 0, 0);
	return canvas;
};

var getImageDataAsImage = (imgData, width, height, onload) => {
	const canvas = drawImageDataOnNewCanvas(imgData, width, height);
	const dataURL = canvas.toDataURL();
	const img = new Image(width, height);
	// img.onload = event => onload(event, img);
	img.width = width;
	img.height = height;
	img.src = dataURL;
	return img;
};

export {
	decode,
	drawImageDataOnNewCanvas,
	getImageDataAsImage
};

