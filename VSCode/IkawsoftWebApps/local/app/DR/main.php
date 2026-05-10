<?php

    require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

    mIncludePage(enuIncludePackageID::InitConfig, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));    

?>

<!DOCTYPE html>
<html lang="en">
 <head>
   <title>Disaster Recovery Tracker</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <?php echo(mOutputHTMLHead()); ?>
 
 </head>
 
 <body style="margin-top: 0px; margin-left: 0px;">
    
        <?php

            $sIncludeFilePath = mGetRootPath(ApplicationID::DR, enuPathRootType::File, enuPathType::ApplicationFolder, enuFolderType::ui) . 'header.php';
            require_once $sIncludeFilePath;

        ?>

        <div class="table-container">
                
                    <div id ="DivAddTable" style="display:none;">
                        <table id="AddTable" class="table table-bordered" style="width:100%;">
                            <thead class="table-header">
                            </thead>
                            <tbody id="tableBody">
                            </tbody>
                        </table>
                    </div>

                    <!-- Sub Select Drop Down-->
                    <div id ="DivSubSelect" style="display:none;background-color: #E5F5FB;" class="card mb-1 border-0 shadow-sm">
                        <div class="card-body py-2 d-flex align-items-center gap-3">
                            <span class="fw-bold">Please Select An Option:</span>
                            <span class="DropDownMessage"></span>
                            <select name="cboSubSelect" id="cboSubSelect" class="form-select form-select-sm w-auto" style="width: auto;min-width: fit-content;"></select>
                        </div>
                    </div>                    


                    <div id ="DivTestSummary" class="table-responsive" style="display:none;">
                        <table id="TestSummaryTable" name="TestSummaryTable" class="table table-bordered mb-0 table-sm text-center" style="display:none;">
                            <thead class="table-light">
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </div>                    
               
                   <!-- Standard Table Header Section -->
                    <div id="DivStandardTableHeaderSection" class="d-flex justify-content-between align-items-center mb-4" style="display:none;margin-top:20px;margin-left:0px;margin-right:0px">
                            <!-- Table/Calendar Icon + The Title/Message At the Top of the Table -->
                            <h5 class="mb-0">
                                <i id="StandardTableHeaderTableIcon" class="bi bi-table mr-2" style="margin-left: 20px;margin-right: 5px;display:none;"></i> <span id="StandardTableTitle" class=""></span>
                            </h5>

                            <!-- Container DIV -->
                            <div class="d-flex align-items-center gap-3">

                                <!-- Search Row DIV With Magnifying Flass and Search Div-->
                                <div class="position-relative search-wrapper" style="min-width: 300px;">
                                    <i  id="StandardTableHeaderSearchIcon" class="bi bi-search position-absolute top-50 start-0 translate-middle-y ms-3 text-muted" style="display:none;"></i>
                                    <div id="SearchRow" style="display: none;"><input type='text' class='form-control ps-5 rounded-pill border py-2 search-input' name='txtSearchCriteria' id='txtSearchCriteria' placeholder='Search entries...' style='background: #f8fafc;' /></div>
                                </div>

                                <!-- Toolbar Table Table -->
                                <table id="ToolbarTable" name="ToolbarTable" class="table mb-0 table-sm text-center">
                                    <tbody>
                                    </tbody>
                                </table>

                            </div>
                    </div>     
                

                <!-- Standard Table -->       
                <fieldset id="FstLockStandardTable">    
                    <table id="StandardTable"  class="table table-bordered table-hover">
                        <thead class="table-header">
                        </thead>
                        <tbody class="table-body">
                        </tbody>
                    </table>
                </fieldset>
        </div>

        <?php
            mIncludeStatusBar($_SERVER['REQUEST_URI'], mGetApplicationID($_SERVER['REQUEST_URI'], true), $_SESSION["FullUserName"], $_SESSION["UserTypeDescription"]);
        ?>

        <script>


            document.addEventListener("DOMContentLoaded", async function () {
                /*Events Fired by the Main Page Such as the loading of the page */
    
                <?php  

                    $iPHPPlatformID = $_SESSION['SelectedPlatform'];
                    $iPHPPlatformID = htmlspecialchars($iPHPPlatformID, ENT_QUOTES);

                    $iPHPYearID = $_SESSION['SelectedYear'];
                    $iPHPYearID = htmlspecialchars($iPHPYearID, ENT_QUOTES);

                    $iPHPPageID = $_SESSION['SelectedPage'];
                    $iPHPPageID = htmlspecialchars($iPHPPageID, ENT_QUOTES);

                    $iPHPUserTypeID = $_SESSION['UserType'];
                    $iPHPUserTypeID = htmlspecialchars($iPHPUserTypeID, ENT_QUOTES);                    

                    $iPHPUserID = $_SESSION['UserID'];
                    $iPHPUserID = htmlspecialchars($iPHPUserID, ENT_QUOTES);  
                ?>                

                try {
                    
                    let iUserID = "<?php echo $iPHPUserID; ?>";
                    let iUserTypeID = "<?php echo $iPHPUserTypeID; ?>";
                    let iPlatformID = "<?php echo $iPHPPlatformID; ?>";
                    let iYearID = "<?php echo $iPHPYearID; ?>";
                    let iPageID = "<?php echo $iPHPPageID; ?>";
                    
                    iUserID = Number(iUserID);
                    iUserTypeID = Number(iUserTypeID);
                    iPlatformID = Number(iPlatformID);
                    iYearID = Number(iYearID);
                    iPageID = Number(iPageID);

                    await mHandleMainPageEvent(iPlatformID, iYearID, iPageID, iUserID, iUserTypeID);
                    
                } catch (oError) {
                    const sFunctionName = "Main.php document ready function.";
                    mSetStatus(sFunctionName, oError);
                }


                document.querySelectorAll("#cboPlatform, #cboYear, #cboPage").forEach(function(oControl) {
                    oControl.addEventListener("change", async function () {
                        
                        try {
                            
                            let iSelectedPlatformID = document.getElementsByName("cboPlatform")[0].value;
                            let iSelectedYearID = document.getElementsByName("cboYear")[0].value;
                            let iSelectedPageID = document.getElementsByName("cboPage")[0].value;
                            let iCurrentUserID = "<?php echo $_SESSION['UserID'] ?>";      
                            let iCurrentUserTypeID = "<?php echo $_SESSION['UserType'] ?>";                    
                            let iSelectedTestID = document.getElementsByName("cboSubSelect")[0]?.value ?? -1;
                            let bUtilityPageLoaded = false;

                            iSelectedPlatformID = Number(iSelectedPlatformID);
                            iSelectedYearID = Number(iSelectedYearID);
                            iSelectedPageID = Number(iSelectedPageID);
                            iCurrentUserID = Number(iCurrentUserID);
                            iCurrentUserTypeID = Number(iCurrentUserTypeID);
                            iSelectedTestID  = Number(iSelectedTestID);
                            
                            bUtilityPageLoaded = mOpenUtilityPage(iSelectedPageID);

                            if (bUtilityPageLoaded === false) {
                                
                                await mHandleMainPageEvent(iSelectedPlatformID, iSelectedYearID, iSelectedPageID, iCurrentUserID, iCurrentUserTypeID, iSelectedTestID);

                            }

                        } catch (oError) {
                            const sFunctionName = "cboYear/Page Change Function";
                            mSetStatus(sFunctionName, oError);
                        }
                    });
                });
            });


            function mOpenUtilityPage(iPageID) {
                try {

                    let bUtilityPageLoaded = false;
                    let sDestinationURL = "";
                    let sQueryStringVariable = "";
                    let sQueryStringOperatorStart = "";
                    let oDestination = [];

                    oDestination = arrUtilityPages.find(arrItemPageID => arrItemPageID.PageID === iPageID);

                    if (!oDestination) {
                        bUtilityPageLoaded = false;

                    } else {

                        if (oDestination.QueryStringVariable.trim().length === 0) {
                            sQueryStringOperatorStart = "?"
                        } else if (oDestination.QueryStringVariable.trim().length > 0) {
                            sQueryStringOperatorStart = "&"
                        }

                        sQueryStringVariable = oDestination.QueryStringVariable + sQueryStringOperatorStart + "Session_Name=Disaster_Recovery" ;
                        sQueryStringVariable = sQueryStringVariable + "&AppID=" + giAppID;
                        sDestinationURL = oDestination.URLLocation + sQueryStringVariable;

                        window.open(sDestinationURL, "_blank");
                        bUtilityPageLoaded = true;

                    }

                    return(bUtilityPageLoaded);

                } catch (oError) {
                    mSetStatus("mOpenUtilityPage", oError);
                }
            }


        </script>   

    <?php
        mIncludeJSInclude($_SERVER['REQUEST_URI'], -1, true);
    ?>
   
    


    

 </body>
</html>

