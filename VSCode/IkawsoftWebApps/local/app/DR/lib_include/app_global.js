        const cstColumnID = {
            RowID: 0,
            RowType: 1,
            StartTime: 2,
            EndTime: 3
        };

        const cstDateComparisonMode = {
            Min: 0,
            Max: 1
        };


	const cstUserType = {
		Standard: 5510,
		CrossPlatform: 5515,
		Admin: 5520,
	};

	const cstGetRecordsetAction = {
		Design: 1,
		Component: 2,
		Lookup: 3,
		DB_Table_Schema: 4,
		New_Sequence_ID: 5,
		Documentation: 6,
		Infrastructure: 6,
		"Prep Tasks": 6,
		Inventory: 6,
		"Issue Log": 6,
		Metric: 12,
		Test: 13,
		New_Test_ID: 14,
		"Hour By Hour Plan": 15,
		"Check_Test_From_HxH_Plan": 16,
		"Test_Summary": 17,
		"Design Diagram": 18,
		"Test_Type": 19,
		"Application ID": 20,
		"User": 22,
		"HTMLControl": 23,
		"PageRow": 24,
		"AllTests": 25,
		"New_Row_ID": 26,
		"Diagram": 6,		
		"Test_Settings": 27,	
		"Test_Applications": 28,					
		"Plan Template": 29,				
		"Check_Plan_Lock": 30,						
		"Artifact": 31,
		"All_Artifact": 32,		
	};


	const cstTableName = {
		Design: "tb_Design",
		Documentation: "tb_Documentation",
		Infrastructure: "tb_Infrastructure",
		"Prep Tasks": "tb_Prep_Task",
		"Inventory": "tb_Inventory",
		"Hour By Hour Plan": "tb_Plan",
		"Issue Log": "tb_Issue_Log",
		"Metric": "tb_Metric",
		"Test": "tb_Test",
		"User": "[Ikawsoft_Central].[dbo].tb_User",
		"Diagram": "tb_Design_Diagram",
		"Plan Backup": "tb_Plan_Backup",
		"Plan Template": "tb_Plan_Template",
		"Plan Template Backup": "tb_Plan_Template_Backup",
		"Artifact": "tb_Artifact",
	};
	
	const cstElementID = {
		DivAddTable: "DivAddTable",
		DivSubSelect: "DivSubSelect",
		DivTestSummary: "DivTestSummary",
		DivStandardTableHeader: "DivStandardTableHeaderSection",
		FstStandardTable: "FstLockStandardTable",
	};

	const cstPageBitLookup = {
		120: 0,
		125: 1,
		130: 2,
		135: 3,
		140: 4,
		145: 5,
		150: 6,
		155: 7,
		5470: 8,
		5500: 9,
		5505: 10
	};

	const cstTaskStatus = {
		NotStarted: 230,
		InProgress: 235,
		Complete: 240,
		NotRequired: 5400
	};

	let oBroadcastClient = null;
	let bIsWebSocketReconnecting = false;

	let arrHTMLControlTable = [];
	let arrHTMLNoButtonControlTable = [];
	let arrLookupTable = [];
	//let arrComponentLookupTable = [];
	//let arrDatabaseSchema = [];


	//Application Variables
	let gsApplicationEnvironment = "";
	let gsClientID = "";
	let gsApplicationNames = "";	
	let gsPageName = "";
	let gsTableName = "";	
	let gsErrorMessage = "";
	let giAppID = 1;	
	let giPlatformID = -1;
	let gsPlatform = "";
	let giYearID = 0;
	let giPageID = 0;
	let giTestTypeID = -1;
	let giTestID = -1;
	let gbTestLock = false;
	let giRowID = -1;
	let gbFirstError = true;
	
	//Plan Variables
	let giPlanStartDate = "";
	let giPlanStartTime = "";
	let giPlanStartTime24Hr = "";

	//User Variables
	let giUserID = -1;
	let giUserTypeID = -1;
	let gsUserType = "";

	//Application Dictionaries
	let dctPlatform = {};
	let dctYear = {};
	let dctPage = {};
	let dctUserType = {};




