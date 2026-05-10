<?php

   if (session_status() === PHP_SESSION_NONE) {
      session_name("Disaster_Recovery");
      session_start();
   }


   try {
         require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

         mIncludePage(enuIncludePackageID::InitConfig, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));   
         
         $iPlatformID = -1;
         $iYearID = -1;
         $iTestID = -1;
         $iRowID = -1;


         $arrExpectedValue = ["PlatformID", "YearID", "TestID", "RowID"];
         $values   = [];

         foreach ($arrExpectedValue as $vGetVariable) {
            if (!isset($_GET[$vGetVariable])) {
               throw new Exception("Request Method variables are required.  " . print_r($arrExpectedValue, true));
            }
         }    

         if (isset($_GET["PlatformID"])) {
            $iPlatformID = (int)$_GET["PlatformID"];
         }

         if (isset($_GET["YearID"])) {
            $iYearID = (int)$_GET["YearID"];
         }

         if (isset($_GET["TestID"])) {
            $iTestID = (int)$_GET["TestID"];
         }

         if (isset($_GET["RowID"])) {
            $iRowID = (int)$_GET["RowID"];
         }
      } catch (Throwable $oError) {
          throw $oError;
      }  
?>

<!DOCTYPE html>
<html lang="en">
 <head>
   <title>Disaster Recovery Artifacts</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <?php echo(mOutputHTMLHead()); ?>
 

   <style>
      #divArtifactPaste {
         background-color: #ffffff;      /* white background */
         border: 1px solid #ccc;         /* light border like a textbox */
         border-radius: 4px;             /* optional: rounded corners */
         padding: 8px;                   /* space inside the box */
         min-height: 120px;              /* so it looks like a paste area */
         outline: none;                  /* remove blue outline on click */
         cursor: text;                   /* text cursor on hover */
      }

      #divArtifactPaste:focus {
         border-color: #66afe9;          /* highlight on focus */
         box-shadow: 0 0 4px rgba(102,175,233,0.6);
      }
   </style>    

   
 </head>
 
 <body style="margin-top: 0px; margin-left: 0px;">
    <div style="text-align:right; padding-right:35px;padding-top:20px;">
      <button name="btnSaveArtifact" id="btnSaveArtifact" value="" onclick="mSaveArtifact(gliSaveType)" class="btn btn-custom btn-primary rounded-pill px-4 border-0 d-inline-flex align-items-center gap-2 py-2 use-element-alignment" title="Lock Scores" style="width: 165px; text-align: center !important; text-align-last: center !important;">
         <i class="bi bi-plus-lg"></i><span class="d-none d-sm-inline font-weight-bold">Save Artifact</span>
      </button>
   </div>
   <br>

   <div id="divArtifactPaste" contenteditable="true">
      Paste your new artifact image here.  It will be added to the bottom of this window as the next artifact in the sequence for this DR task.
   </div>

    <?php

        
    
    ?>

  
   <br>
   <br>

    <?php
        mIncludeStatusBar($_SERVER['REQUEST_URI'], mGetApplicationID($_SERVER['REQUEST_URI'], true), $_SESSION["FullUserName"], $_SESSION["UserTypeDescription"]);    
        mIncludeJSInclude($_SERVER['REQUEST_URI'], -1, false);
    ?>

      <script>
               
         let oArtifactLabelSpan = null;
         let gliPlatformID = <?= $iPlatformID ?>;
         let gliYearID = <?= $iYearID ?>;
         let gliTestID = <?= $iTestID ?>;
         let gliRowID = <?= $iRowID ?>;
         let gliImageCount = 0;
         let gliArtifactID = -1; 
         let gliSaveType = -1;      

         const oArtifactPastDiv = mGetHTMLElement(cstGetHTMLElementType.ID, "divArtifactPaste")
         const cstDivArtifactMessagePrefix = "Paste your new artifact image here.";

         document.addEventListener('paste', (oEvent) => {

            try {

               let oClipboardItems = null;

               const oDivArtifactPaste = document.getElementById("divArtifactPaste");
               oClipboardItems = oEvent.clipboardData.items;

               if (oDivArtifactPaste.textContent.includes(cstDivArtifactMessagePrefix)) {
                  oDivArtifactPaste.innerHTML = "";
               }

               for (const oClipboardItem of oClipboardItems) {

                  //Checks if clipboard contains in image type file
                  if (oClipboardItem.type.startsWith('image/')) {

                     oEvent.preventDefault(); // stop browser from inserting anything

                     const oImageBlob = oClipboardItem.getAsFile();
                     const sImageURL = URL.createObjectURL(oImageBlob);

                     // Insert new image
                     const oImage = document.createElement("img");
                     oImage.src = sImageURL;
                     oImage.style.maxWidth = "100%";
                     oImage.style.height = "auto";


                     // Create the Image Label
                     gliImageCount = gliImageCount + 1;


                     //Output the Text in a Span
                     oArtifactLabelSpan = mGetArtifactLabelSpan(gliTestID, gliRowID, gliImageCount);
                     

                     //Output the Label and the Image
                     oDivArtifactPaste.appendChild(document.createElement("br"));
                     oDivArtifactPaste.appendChild(document.createElement("br"));                    
                     oDivArtifactPaste.appendChild(oArtifactLabelSpan);                     
                     oDivArtifactPaste.appendChild(oImage);

                  }

               }

            } catch (oError) {
               mSetStatus("paste_event", oError);
            }    
         });

         document.addEventListener("DOMContentLoaded", async function () {
            try {

               mPopulateArtifact();

            } catch (oError) {
               mSetStatus("DOMContentLoaded_artifact.php", oError);
            }            

         });

         
         oArtifactPastDiv.addEventListener("focus", function () {
            try {

               if (oArtifactPastDiv.textContent.includes(cstDivArtifactMessagePrefix)) {
                  oArtifactPastDiv.textContent = "";
               }

            } catch (oError) {
               mSetStatus("oArtifactPastDiv.addEventListener", oError);
            }             
         });





         async function mPopulateArtifact() {
            try {

               let arrRecordset = [];
               let sArtifactContent = "";
               let oArgumentData = new FormData();              
               let oImageDiv = null;

               oArgumentData.append("ActionID", cstGetRecordsetAction["Artifact"]);
               oArgumentData.append("PlatformID", gliPlatformID);
               oArgumentData.append("YearID", gliYearID);
               oArgumentData.append("TestID", gliTestID);
               oArgumentData.append("RowID", gliRowID);
               
               arrRecordset = await mGetRecordset(cstURL.GetRecordset, oArgumentData);

                  if (Array.isArray(arrRecordset) && arrRecordset.length > 0) {
                        oImageDiv =  mGetHTMLElement(cstGetHTMLElementType.ID, "divArtifactPaste");

                        sArtifactContent = String(arrRecordset[0].Artifact_Content);

                        gliImageCount = await mOutputArtifact(sArtifactContent, oImageDiv);

                        gliArtifactID = arrRecordset[0].Artifact_ID;
                        gliSaveType = cstCommandType.Update;
                        
                  } else {

                        gliSaveType = cstCommandType.Insert;

                  }


               arrRecordset = null;

            } catch (oError) {
               mSetStatus("mPopulateArtifact", oError);
            }                
         }         




         async function mSaveArtifact(iSaveType) {
            try {

               let sDivContent = "";
               let arrFieldName = [];
               let arrFieldValue = [];
               let oExecuteSQLResult = null;
               let oArtifactDiv = null;

               oArtifactDiv = document.getElementById("divArtifactPaste");

               if (iSaveType === cstCommandType.Insert) {
                     arrFieldName.push("Platform_ID");
                     arrFieldName.push("Year_ID");
                     arrFieldName.push("Test_ID");
                     arrFieldName.push("Row_ID");
                     arrFieldName.push("Artifact_Content");

                     arrFieldValue.push(gliPlatformID);
                     arrFieldValue.push(gliYearID);
                     arrFieldValue.push(gliTestID);
                     arrFieldValue.push(gliRowID);
                     sDivContent = await mGetDivContentAsString(oArtifactDiv);
                     arrFieldValue.push(sDivContent);

               } else if (iSaveType === cstCommandType.Update) {

              
                     arrFieldName.push("Artifact_Content");
                     arrFieldName.push("Artifact_ID");

                     sDivContent = await mGetDivContentAsString(oArtifactDiv);
                     arrFieldValue.push(sDivContent);
                     arrFieldValue.push(gliArtifactID);

               } else {
                  const oLocalError = new Error();
                  oLocalError.AlertMessage = "Artifact could not be saved.  Save Type is not specified.  iSaveType:  " + iSaveType;
                  throw oLocalError;
               }


               oExecuteSQLResult = await mCallExecuteSQL(cstURL.ExecuteSQL, iSaveType, cstTableName["Artifact"], arrFieldName, arrFieldValue);

               if (oExecuteSQLResult.error === true) {
                  throw new Error(oExecuteSQLResult.message);
               } else {
                  mPopulateArtifact();
                  alert("Artifact saved successfully.")
               }

               oArtifactDiv = null;

            } catch (oError) {
               mSetStatus("mSaveArtifact", oError);
            }                
         }


      </script>    
   </body>

 
</html>    