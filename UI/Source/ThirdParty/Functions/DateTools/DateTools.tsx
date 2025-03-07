/**
 * Date ordinal for the given index. e.g. 1 is '1st', 2 is '2nd' etc.
 * @param i
 * @returns
 */
function ordinal(i: int): string {
    var j = i % 10,
        k = i % 100;
    if (j == 1 && k != 11) {
        return i + `st`;
    }
    if (j == 2 && k != 12) {
        return i + `nd`;
    }
    if (j == 3 && k != 13) {
        return i + `rd`;
    }
    return i + `th`;
}

const dayNames = [`Sunday`, `Monday`, `Tuesday`, `Wednesday`, `Thursday`, `Friday`, `Saturday`];
const shortDayNames = [`Sun`, `Mon`, `Tue`, `Wed`, `Thu`, `Fri`, `Sat`];

const monthNames = [`January`, `February`, `March`, `April`, `May`, `June`, `July`, `August`, `September`, `October`, `November`, `December`];
const shortMonthNames = [`Jan`, `Feb`, `Mar`, `Apr`, `May`, `Jun`, `Jul`, `Aug`, `Sep`, `Oct`, `Nov`, `Dec`];

const epochTicks = 62135596800000;
const ticksPerMillisecond = 10000;

/**
 * Converts a JS date to a number of C# ticks.
 * @param date
 * @returns
 */
const ticks = (date : Date) => {
	return epochTicks + (date.getTime() * ticksPerMillisecond);
}

/**
 * Converts a variety of date formats in to a JS Date. All are expected to be UTC.
 * @param isoish
 * @returns
 */
const isoConvert = (isoish : Dateish) : Date => {
	var type = typeof isoish;
	
	if(type == 'number'){
		// milliseconds from year 0
		return new Date((isoish as number) - epochTicks);
	}
	
	if(type != 'string'){
		// already a date
		if(isoish && isoish.valueOf){
			return new Date(isoish.valueOf());
		}
		return isoish as Date;
	}
	
	 // Split the string into an array based on the digit groups.
	var dateParts = (isoish as string).split(/\D+/);

	 // Set up a date object with the current time.
	 var returnDate = new Date();
 
	 // Manually parse the parts of the string and set each part for the
	 // date. Note: Using the UTC versions of these functions is necessary
	 // because we're manually adjusting for time zones stored in the
	 // string.
	// The month numbers are one "off" from what normal humans would expect
	 // because January == 0.
	 returnDate.setUTCFullYear( parseInt( dateParts[ 0 ] ), parseInt( dateParts[ 1 ] ) - 1, parseInt( dateParts[ 2 ] ));
 
	 // Set the time parts of the date object.
	 returnDate.setUTCHours( parseInt( dateParts[ 3 ] ), parseInt( dateParts[ 4 ] ), parseInt( dateParts[ 5 ] )  );
 
	 // Track the number of hours we need to adjust the date by based
	 // on the timezone.
	 var timezoneOffsetHours = 0;
 
	 // If there's a value for either the hours or minutes offset.
	 if ( dateParts[ 7 ] || dateParts[ 8 ] ) {
 
		 // Track the number of minutes we need to adjust the date by
		 // based on the timezone.
		 var timezoneOffsetMinutes = 0;
 
		 // If there's a value for the minutes offset.
		 if ( dateParts[ 8 ] ) {
 
			 // Convert the minutes value into an hours value.
			 timezoneOffsetMinutes = parseInt( dateParts[ 8 ] ) / 60;
		 }
 
		 // Add the hours and minutes values to get the total offset in
		 // hours.
		 timezoneOffsetHours = parseInt( dateParts[ 7 ] ) + timezoneOffsetMinutes;
 
		 // If the sign for the timezone is a plus to indicate the
		 // timezone is ahead of UTC time.
		 if ((isoish as string).slice(-6, -5) == "+" ) {
 
			 // Make the offset negative since the hours will need to be
			 // subtracted from the date.
			 timezoneOffsetHours *= -1;
		 }
         
         returnDate.setUTCHours( parseInt( dateParts[ 3 ] ) + timezoneOffsetHours );
         returnDate.setUTCMinutes( parseInt( dateParts[ 4 ] )  + timezoneOffsetMinutes );
	 }
	
	 // Return the Date object calculated from the string.
	 return returnDate;
}

/**
 * Converts a local date to UTC.
 * @param date
 * @returns
 */
function localToUtc(date : Date){
	return new Date(Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
	date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds()));
}

/**
 * Converts a UTC date to local.
 * @param date
 * @returns
 */
function utcToLocal(date : Date) {
    return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(),  date.getHours(), date.getMinutes(), date.getSeconds()));
}

/**
 * Gets the most recent previous monday.
 * @param date
 * @returns
 */
function getMonday(date: Date) {
  var d = new Date(date);
  var day = d.getDay(),
      diff = d.getDate() - day + (day == 0 ? -6:1); // adjust when day is sunday
  return new Date(d.setDate(diff));
}

/**
 * Gets the most recent previous sunday.
 * @param date
 * @returns
 */
function getSunday(date: Date) {
  var d = new Date(date);
  var day = d.getDay(),
      diff = d.getDate() - day;
  return new Date(d.setDate(diff));
}

/**
 * Adds n days to the given date, returning the updated date.
 * @param date
 * @param days
 * @returns
 */
function addDays(date: Date, days : float) {
    var newDate = new Date(date.valueOf());
	newDate.setDate(date.getDate() + days);
	return newDate;
}

/**
 * Adds n hours to the given date, returning the updated date.
 * @param date
 * @param days
 * @returns
 */
function addHours(date : Date, hours : float) {
	return addMinutes(date, hours * 60);
}

/**
 * Adds n minutes to the given date, returning the updated date.
 * @param date
 * @param days
 * @returns
 */
function addMinutes(date : Date, minutes : float) {
	var date = new Date(date.valueOf() + (1000 * 60 * minutes));
	return date;
}

/**
 * Adds n seconds to the given date, returning the updated date.
 * @param date
 * @param days
 * @returns
 */
function addSeconds(date : Date, seconds : float) {
	var date = new Date(date.valueOf() + (1000 * seconds));
	return date;
}

/**
 * Gets the number of days until the given date. Can be negative.
 * @param date
 * @param days
 * @returns
 */
function daysUntilDate(date: Dateish) : float {
	var start = isoConvert(date);
	var currentTimeUTC = new Date();
	var diff = start.getTime() - currentTimeUTC.getTime();
	var days = Math.ceil(diff / (1000 * 3600 * 24));

	switch (days) {
		case 0:
		case 1:
			return start.setHours(0, 0, 0, 0) == currentTimeUTC.setHours(0, 0, 0, 0) ? 0 : 1;

		default:
			return days;
	}
}

/**
 * The number of days between the two given dates.
 * @param startdate
 * @param enddate
 * @returns
 */
function daysBetween(startdate: Dateish, enddate: Dateish) : float {
	var start = isoConvert(startdate).setHours(0, 0, 0, 0);
	var end = isoConvert(enddate).setHours(0, 0, 0, 0);	
	var diff = end - start;
	var days = Math.ceil(diff / (1000 * 3600 * 24));

	return days;
}

/**
 * The number of minutes between the two given dates.
 * @param startdate
 * @param enddate
 * @returns
 */
function minsBetween(startdate: Dateish, enddate: Dateish): float{
	var start = isoConvert(startdate);
	var end = isoConvert(enddate);	
	var diff = Math.abs(end.valueOf() - start.valueOf());
	var mins = Math.floor((diff /1000) /60);
	return mins;
}

/**
 * Displays the given date using the named locales date formatting.
 * @param date
 * @param locales
 * @param options
 * @returns
 */
function toLocaleUTCDateString(date: Date, locales: string, options?: Intl.DateTimeFormatOptions) {
	const timeDiff = date.getTimezoneOffset() * 60000;
	const adjustedDate = new Date(date.valueOf() + timeDiff);
	return adjustedDate.toLocaleDateString(locales, options);
}

/**
 * Displays the given date as only its time using the named locales date formatting.
 * @param date
 * @param locales
 * @param options
 * @returns
 */
function toLocaleUTCTimeString(date: Date, locales: string, options?: Intl.DateTimeFormatOptions) {
	const timeDiff = date.getTimezoneOffset() * 60000;
	const adjustedDate = new Date(date.valueOf() + timeDiff);
	return adjustedDate.toLocaleTimeString(locales, options);
}

/**
 * True if the given dates are the same day.
 * @param d1
 * @param d2
 * @returns
 */
function isSameDay(d1: Date, d2: Date) {
	return d1.getUTCFullYear() === d2.getUTCFullYear() &&
		d1.getUTCMonth() === d2.getUTCMonth() &&
		d1.getUTCDate() === d2.getUTCDate();
}

export {
	ordinal,
	dayNames,
	shortDayNames,
	monthNames,
	shortMonthNames,
	isoConvert,
    utcToLocal,
	localToUtc,
	getMonday,
	getSunday,
	addDays,
	addMinutes,
	addHours,
	addSeconds,
	daysUntilDate,
	daysBetween,
	minsBetween,
	ticks,
	toLocaleUTCDateString,
	toLocaleUTCTimeString,
	isSameDay
};
