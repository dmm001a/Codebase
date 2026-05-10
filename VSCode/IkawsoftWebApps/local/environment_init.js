    //Build Variables for URL Pathing
    const cstURL = {
        SetSession: sApplicationURLRoot + "lib_include/set_Session.php",
        BulkDocumentInsert: sApplicationURLRoot + "db_include/bulk_DR_Document_Insert.php",
        LoadPlanTemplate: sApplicationURLRoot + "db_include/Load_Plan_Template.php",
        TogglePlanTimingLock: sApplicationURLRoot + "db_include/Toggle_Plan_Timing_Lock.php",
        InsertPlanBackup: sApplicationURLRoot + "db_include/Insert_Plan_Backup.php",
        InsertPlanTemplateBackup: sApplicationURLRoot + "db_include/Insert_Plan_Template_Backup.php",
        GetRecordset: sSharedLibraryRootURL + "db_include/get_Recordset.php",
        ExecuteSQL: sSharedLibraryRootURL + "db_include/execute_SQL.php",
        UploadArtifactImage: sSharedLibraryRootURL + "lib_include/srv_upload.php",
        WebSocketServer: "wss://ikawsoft.com/broadcast",
        FormFolder: sApplicationURLRoot + "form",
	    WebServerURL: window.location.origin,
	    FullURL: window.location.href,
	    URLExtension: window.location.pathname,
      };
    
    
    const sApplicationEnvironment = mGetApplicationEnvironment(window.location.href);

    const arrUtilityPages = [
        {
            PageID: 5525,
            URLLocation: sSharedLibraryRootURL + "utilities/" + "error_log.php",
            QueryStringVariable: "?ApplicationFolder=DR",
        },
        {
            PageID: 5530,
            URLLocation: sSharedLibraryRootURL + "utilities/" + "session_tools.php",
            QueryStringVariable: "",
        },
        {
            PageID: 5540,
            URLLocation: sSharedLibraryRootURL + "utilities/" + "send_broadcast_message.php",
            QueryStringVariable: "",
        },
        {
            PageID: 5555,
            URLLocation: sSharedLibraryRootURL + "lib_include/" + "js_include.php",
            QueryStringVariable: "",
        },
        {
            PageID: 5560,
            URLLocation: sSharedLibraryRootURL + "utilities/" + "sql_view.php",
            QueryStringVariable: "",
        },       
        {
            PageID: 5655,
            URLLocation: sSharedLibraryRootURL + "utilities/" + "phpinfo_plus.php",
            QueryStringVariable: "",
        } 
    ];



    function mAlertErrorMessage() {

        if (gsErrorMessage.length > 0) {
            setTimeout(() => {
                alert("Error: " + gsErrorMessage);
                gsErrorMessage = "";
            }, 1);
        }

    }


    function mErrorNumberProcessing(iErrorNumber) {
        try {
            
            let sDisplayMessage = "";
            
            if (iErrorNumber) {
            
            switch (iErrorNumber) {
                
                case 2627: // Primary key violation
                sDisplayMessage = "You have attempted to add or update a record with a matching record for the primary key of this table.  This attempt was cancelled."
                
                case 2601:// Unique index violation
                case 547: // Foreign key violation
                sDisplayMessage = "You have attempted to add or update a record with a matching record for the unique or foreign key of this table.  This attempt was cancelled."
            }
            
            }
            
            return(sDisplayMessage);
        
        } catch (oError) {
            mSetStatus("mErrorNumberProcessing", oError);
        }	
    }