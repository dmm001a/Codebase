    function mBuildString(sCurrentValue, sNewValue, cAddToString = cstAddToString.Nothing) {
        try {

            let sReturnString = "";

            if ((cAddToString === cstAddToString.Nothing) && (sNewValue.length > 0)){
                sReturnString = sCurrentValue + " " + sNewValue;
            } else if ((cAddToString === cstAddToString.Nothing) && (sNewValue.length === 0)){
                sReturnString = sCurrentValue;                
            } else if (cAddToString === cstAddToString.SingleQuotes) {
                sReturnString = sCurrentValue + " '" + sNewValue + "'";
            } else if (cAddToString === cstAddToString.DoubleQuotes) {
                sReturnString = sCurrentValue + ' "' + sNewValue + '"';
            } else if (cAddToString === cstAddToString.EndSemiColon) {
                sReturnString = sCurrentValue + " " + sNewValue + ";";                
            } else if (cAddToString === cstAddToString.CarriageReturn) {                
                sReturnString = sCurrentValue + " " + sNewValue + "\n";
            } else if (sCurrentValue.length === 0) {
                sReturnString = sNewValue;
            }


            return sReturnString;

        } catch (oError) {
           mSetStatus("mBuildString", oError);
        }
    }

    function mCheckTableTypeMask(iTableTypeMask, iTableTypeBit) {
        try {

            return (iTableTypeMask & iTableTypeBit) !== 0;

        } catch (oError) {
            mSetStatus("mCheckTableTypeMask", oError);
        } 

    }


    function mToggleBetweenBitBoolean(vValue) {
        try {

            let vReturnValue = null;

            if (mIsBoolean(vValue) === true) {
                if (vValue === true) {
                    vReturnValue = 1;
                } else if (vValue === false) {
                    vReturnValue = 0;
                }
            } else if (mIsBit(vValue) === true) {
                if (Number(vValue) === 1) {
                    vReturnValue = true;
                } else if (Number(vValue) === 0) {
                    vReturnValue = false;
                }
            } else {
                vReturnValue = null;
            }

            return(vReturnValue);

        } catch (oError) {
            mSetStatus("mToggleBetweenBitBoolean", oError);
            vReturnValue = null;            
        } 

    }

    function mIsBoolean(vValue) {
        try {

            let bIsBoolean = false;

            if (vValue !== null) {
                if (vValue === true || vValue === false) {
                    bIsBoolean = true;
                }
            }

            return(bIsBoolean);

        } catch (oError) {
            mSetStatus("mIsBoolean", oError);
            return(false);            
        } 
    }

    function mIsBit(vValue) {
        try {

            let bIsBit = false;

            if (mIsNumeric(vValue)) {
                if (Number(vValue) === 0 || Number(vValue) === 1) {
                    bIsBit = true;
                }
            }

            return(bIsBit);

        } catch (oError) {
            mSetStatus("mIsBit", oError);
            return(false);
        } 
    }    

    function mConvertTimeToSqlTimeFormat(tInputTime) {

        let dTempDate = null;
        let vHours = "";
        let vMinutes = "";
        let vSeconds = "";

        try {                  

            dTempDate = new Date("1970-01-01 " + tInputTime);

            if (isNaN(dTempDate)) return null; // invalid input

            vHours = dTempDate.getHours().toString().padStart(2, "0");
            vMinutes = dTempDate.getMinutes().toString().padStart(2, "0");
            vSeconds = dTempDate.getSeconds().toString().padStart(2, "0");

            return `${vHours}:${vMinutes}:${vSeconds}`; // SQL Server time(7) compatible

        } catch (oError) {
            mSetStatus("mConvertTimeToSqlTimeFormat", oError);
        }                
    }


    function mIsNull(oValue) {
        try {        

            return oValue === null || oValue === undefined;

        } catch (oError) {
            mSetStatus("mIsNull", oError);
        }            
    }

    function mIsNumeric(vValue) {
        try {
            let bIsNumeric = false;
            let nValue = null;

            if (String(vValue).trim().length === 0) {
                
                bIsNumeric = false;

            } else {

                // Convert to number explicitly
                nValue = Number(vValue);

                // Check if the conversion produced a real, finite number
                if (Number.isFinite(nValue)) {
                    bIsNumeric = true;
                } else {
                    bIsNumeric = false;
                }
            }

            return bIsNumeric;

        } catch (oError) {
            mSetStatus("mIsNumeric", oError);
        }
    }


    function mIsString(vValue) {
        try {        

            let bIsString = false;

            bIsString = typeof vValue === "string";

            return(bIsString);

        } catch (oError) {
            mSetStatus("mIsString", oError);
        }
    }

    function mAddNewLine(sCurrentLine, sNewLine) {

        let sReturnString = "";

        try {

            sReturnString = sCurrentLine + "\n" + sNewLine;
            
            return(sReturnString);

        } catch (oError) {
           mSetStatus("mAddNewLine", oError);
        }
    }

    function mIsEmpty(vInputValue) {

        try {

            if (vInputValue == null) return true;                 // null or undefined
            if (Array.isArray(vInputValue) && vInputValue.length === 0) return true; // empty array
            if (typeof vInputValue === "object" && Object.keys(vInputValue).length === 0) return true; // empty object

            return false;

        } catch (oError) {
            mSetStatus("mIsEmpty", oError);
            return false;
        }

    }


    function mRandomInt(iMaximumInteger) {
        
        try {        
        
            return Math.floor(Math.random() * iMaximumInteger) + 1;

        } catch (oError) {
            mSetStatus("mRandomInt", oError);
        }        
    }

    function mGetCurrentCentralTime(b12Hour, bWithAMPM) {

        try {

            const oOptions = {
                timeZone: 'America/Chicago',
                hour: '2-digit',
                minute: '2-digit',
                hour12: b12Hour === true
            };

            let sCentralTime = new Intl.DateTimeFormat('en-US', oOptions).format(new Date());

            // Remove AM/PM if caller does not want it
            if (b12Hour === true && bWithAMPM === false) {
                sCentralTime = sCentralTime.replace(/\s?(AM|PM)$/i, "");
            }

            return sCentralTime;

        } catch (oError) {
            mSetStatus("mGetCurrentCentralTime", oError);
            return null;
        }
    }

    function mCheckCommaStringForValue(sCommaString, iValueToMatch) {

        try {            

            let bMatch = false;

            const arrItemValues = sCommaString.split(',').map(vItemValue => Number(vItemValue.trim())).filter(Number.isFinite);
            iValueToMatch = Number(iValueToMatch);

            bMatch = arrItemValues.includes(iValueToMatch);

            return(bMatch);

        } catch (oError) {
            mSetStatus("mCheckCommaStringForValue", oError);
            return false;
        }               
    }

    function mGetSQLDateTimeCentral(dtDateTime) {

        try {                        
            const dtConversion = new Date(dtDateTime);

            // Convert to Central Time
            const dtCentral = new Date(dtConversion.toLocaleString("en-US", { timeZone: "America/Chicago" }));

            //Error Checking
            if (isNaN(dtConversion)) {
                throw new Error("Invalid date input");
            }

            const yyyy = dtCentral.getFullYear();
            const mm   = String(dtCentral.getMonth() + 1).padStart(2, "0");
            const dd   = String(dtCentral.getDate()).padStart(2, "0");
            const hh   = String(dtCentral.getHours()).padStart(2, "0");
            const mi   = String(dtCentral.getMinutes()).padStart(2, "0");
            const ss   = String(dtCentral.getSeconds()).padStart(2, "0");

            return `${yyyy}-${mm}-${dd} ${hh}:${mi}:${ss}`;

        } catch (oError) {
            mSetStatus("mGetSQLDateTimeCentral", oError);
            return null;
        }                   
    }

    function mConvertDBTimeToTimeBoxTime(tDBTimeValue) {
        try {

            let sReturnTimeValue = "";
            let sTimeWithoutSeconds = "";
            let arrTimeParts = [];

            if (tDBTimeValue) {

                // Remove AM/PM if present
                tDBTimeValue = tDBTimeValue.replace(/\s?(AM|PM)$/i, "");

                //Remove fractional seconds
                sTimeWithoutSeconds = tDBTimeValue.split(".")[0];   // "07:01:00"

                //Split HH:MM:00 into array values
                arrTimeParts = sTimeWithoutSeconds.split(":");       // ["07","01","00"]

                if (arrTimeParts.length >= 2) {
                    sReturnTimeValue = arrTimeParts[0] + ":" + arrTimeParts[1];   // "07:01"
                }

            }

            return sReturnTimeValue;

        } catch (oError) {
            mSetStatus("mConvertDBTimeToTimeBoxTime", oError);
        }                
    }     

    function mConvertToTrueFalse(sComparisonValue, sTrueValue, sFalseValue) {
        try {

            let bReturnBoolean = null;

            if (sComparisonValue === sTrueValue) {
                bReturnBoolean = true;
            } else if (sComparisonValue === sFalseValue) {
                bReturnBoolean = false;
            } else {
                throw new Error("Comparison Value did not match True Value or False Value.");
            }

            return(bReturnBoolean);

        } catch (oError) {
            mSetStatus("mConvertToTrueFalse", oError);
        }                
    }     

    function mToggleValue(sComparisonValue, sValue1, sValue2) {
        try {

            let sReturnValue = "";

            if (sComparisonValue === sValue1) {
                sReturnValue = sValue2;
            } else if (sComparisonValue === sValue2) {
                sReturnValue = sValue1;
            } else {
                throw new Error("Comparison Value did not match Value 1 or Value 2 for Toggle.");
            }

            return(sReturnValue);

        } catch (oError) {
            mSetStatus("mConvertToTrueFalse", oError);
        }                
    }   


    function mConvertBitToBoolean(iBit) {
        try {

            let bReturnBoolean = null;

            if (iBit === null) {
                bReturnBoolean = false;
            } else {

                iBit = Number(iBit);

                if (iBit === 0) {
                    bReturnBoolean = false;
                } else if (iBit === 1) {
                    bReturnBoolean = true;
                } else {
                    throw new Error("mConvertBitToBoolean: iBit must be 0 or 1");
                }
            }

            bReturnBoolean = Boolean(bReturnBoolean);

            return (bReturnBoolean);

        } catch (oError) {
            mSetStatus("mConvertBitToBoolean", oError);
        }              
    }


async function mBlobToBase64(oBlob) {
    try {

        if (oBlob.type.startsWith("image/")) {
            return new Promise((resolve, reject) => {
                const reader = new FileReader();
                reader.onloadend = () => resolve(reader.result.split(",")[1]); // strip data URL prefix
                reader.onerror = reject;
                reader.readAsDataURL(oBlob);
            });
        }

    } catch (oError) {
        mSetStatus("mBlobToBase64", oError);
    }         
}


async function mBase64ToUint8Array(sBase64) {
    try {    

        return Uint8Array.from(atob(sBase64), c => c.charCodeAt(0));

    } catch (oError) {
        mSetStatus("mBase64ToUint8Array", oError);
    }  
}
