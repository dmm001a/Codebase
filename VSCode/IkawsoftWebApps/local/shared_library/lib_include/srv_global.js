	const cstTableType = {
        StandardTable: 1,
        AddTable: 2,
        FilterTable: 3,
        PlainHTMLTable: 4,
        ToolbarTable: 5,
        TestSummaryTable: 6,
    };

    const cstTableTypeInText = {
        1: "StandardTable",
        2: "AddTable",
        3: "FilterTable",
        4: "PlainHTMLTable",
        5: "ToolbarTable",
        6: "TestSummaryTable",
    };

    const cstControlType = {
        Textbox: 1,
        DropDown: 3,
        Timebox: 4,
        MultiSelectDropDown: 5,
        DatePicker: 7,
        CommentBox: 9,
        DateTimePicker: 11,
        Hidden: 13,
        Label: 14,
        Button: 15,
        Checkbox: 16,		
    };

    const cstAlignInText = {
        1: "left",
        2: "center",
        3: "right",
    };



	const cstAlignSetting = {
		Left: "left",  // 1
		Center: "center", // 2
		Right: "right", // 3
	}

	const cstWidthSetting = {
		Auto: "auto",
		Zero: "0px",
		Small: "50px",
		SmallPlus: "75px",
		Medium: "100px",
		Large: "250px",
	};

	const cstCommandType = {
	  NonSpecified: 0,
	  Insert: 1,
	  Update: 2,
	  Delete: 3,
	  DeleteWhereIn: 4,
	  DeleteWhereNotIn: 5,	  
	};

	const cstExtensibilityType = {
	  SQLInsert: 1,
	  SQLUpdate: 2,
	  SQLDelete: 3,
	  SaveRelatedTable: 4,
	  UpdatePlannedTimes: 5,
	  GetRelatedTableKey: 6,
	  DeleteTest: 7,
	  GetTestID: 8,
	};

	const cstAddToString = {
		Nothing: 0,
		SingleQuotes: 1,
		DoubleQuotes: 2,
		CarriageReturn: 3,
		EndSemiColon: 4
	};

	const cstRowType = {
		Header: 0,
		Detail: 1,
		ColumnHeader: 2,
	};

	const cstHexColorCode = {
		LightBlue: "#e5f5fb",
		DarkBlue: "#2e86c1",
		Black: "#000000",
		White: "#ffffff",
		Silver: "#E8E8E8",
		LightGreen: "#d4edda",
		MediumGrey: "#dcdcdc",
		YellowAlert: "#FFFF33",
		VeryLightGrey: "#F0F0F0",
		SuperLightGrey: "#F2F2F2",
		Red: "#FF0000",
		Purple: "#edc5ee",
	};

    const cstControlSetting = {
        Do_Not_Include: 0,
        Blank: 1, 
        Text: 2,
        Filter: 3,
        Cell_Invisible: 4,
        Control_Invisible: 5,
        Control_Visible: 6,
        Control_Read_Only_Visible: 7,
    };

	const cstImagePath = {
		GreenLight: "ui_include/images/green_circle_no_background.png",
		RedLight: "ui_include/images/red_circle_no_background.png",
		GreyLight: "ui_include/images/grey_circle_no_background.png",
		BlackLight: "ui_include/images/black_circle_no_background.png",
	};

	const cstHTMLTagType = {
		OpenTableRow: 1,
		CloseTableRow: 2,
		OpenTableCell: 3,
		CloseTableCell: 4,
	};

	const cstHTMLObjectBitLookup = {
		1: 0,  // StandardTableBit
		2: 1,  // AddTableBit
		5: 2   // ToolbarTableBit
	};

	const cstDataType = {
		Raw: 0,
		String: 1,
		Number: 2,
		DateTime: 3,
		Boolean: 4,
	};

	const cstGetHTMLElementType = {
		Name: 1,
		ID: 2
	};