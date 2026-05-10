<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Wolters Kluwer Create Jira</title>
  <style>
    body {
      font-family: Arial, sans-serif;
      background: #2e86c1;
      padding: 40px;
    }
    .form-container {
      background: #fff;
      padding: 20px 30px;
      border-radius: 6px;
      max-width: 500px;
      margin: auto;
      box-shadow: 0 2px 6px rgba(0,0,0,0.1);
    }
    .form-container h2 {
      margin-bottom: 20px;
      font-size: 20px;
      color: #172b4d;
    }
    .form-group {
      margin-bottom: 15px;
    }
    label {
      display: block;
      margin-bottom: 6px;
      font-weight: bold;
      color: #344563;
    }
    input[type="text"],
    input[type="number"],
    textarea,
    select {
      width: 100%;
      padding: 8px;
      border: 1px solid #dfe1e6;
      border-radius: 4px;
      font-size: 14px;
    }
    textarea {
      resize: vertical;
      min-height: 80px;
    }
    button {
      background: #0052cc;
      color: #fff;
      border: none;
      padding: 10px 16px;
      border-radius: 4px;
      cursor: pointer;
      font-size: 14px;
    }
    button:hover {
      background: #0065ff;
    }

    .divFormDetails {
        display: none;
    }

  </style>
</head>

<?php 

    //require_once __DIR__ . '/../lib_include/AzureEmailApiClient.class.php';

      //mSendEmail("DoNotReply@ikawsoft.com", "devon.manelski@wolterskluwer.com", "Test Email", "Test E-mail Body");

     

    if ($_SERVER['REQUEST_METHOD'] === 'POST') {

        // Load Jira credentials from environment variables
        $sJiraDomain = "wkenterprise.atlassian.net"; // e.g., your-domain.atlassian.net
        $sJiraEndpointURL = "https://{$sJiraDomain}/rest/api/3/issue";
        $sJiraEmail  = "devon.manelski@wolterskluwer.com";  // your Atlassian account email
        $sJiraToken  = "Token"; // API token from https://id.atlassian.com/manage-profile/security/api-tokens

        //Init Variables
        $sJiraProject = (string)$_POST["cboJiraProject"];
        $sSummary = (string)$_POST["txtSummary"];
        $sDescription = (string)$_POST["txtDescription"];
        $iJiraType = $_POST["cboJiraType"];
        $iComponent = $_POST["cboComponent"];

        if ($sJiraProject === "WKHL") {
            $iBusinessRank = (int)$_POST["cboBusinessRank"];
            $sBusinessImpacted = (string)$_POST["cboBusinessImpacted"];
        }


        // Jira API endpoint
        

        // Prepare payload
        $oNewJiraData = [
            'fields' => [
                'project' => ['key' => $sJiraProject],
                'summary' => $sSummary,
                'description' => [
                    'type' => 'doc',
                    'version' => 1,
                    'content' => [
                        [
                            'type' => 'paragraph',
                            'content' => [
                                ['type' => 'text', 'text' => $sDescription]
                            ]
                        ]
                    ]
                ],
                'issuetype' => ['id' => $iJiraType],
                'components' => [['id' => $iComponent]],
            ]
        ];

        // Add conditional fields
        if (strtoupper($sJiraProject) === "WKHL") {
            $oNewJiraData['fields']['customfield_10210'] = $iBusinessRank;
            $oNewJiraData['fields']['customfield_10223'] = ['id' => $sBusinessImpacted];
        }

        // Initialize cURL
        $oCurlConnection = curl_init($sJiraEndpointURL);
        curl_setopt($oCurlConnection, CURLOPT_USERPWD, "$sJiraEmail:$sJiraToken");
        curl_setopt($oCurlConnection, CURLOPT_HTTPHEADER, ['Content-Type: application/json']);
        curl_setopt($oCurlConnection, CURLOPT_POST, true);
        curl_setopt($oCurlConnection, CURLOPT_POSTFIELDS, json_encode($oNewJiraData));
        curl_setopt($oCurlConnection, CURLOPT_RETURNTRANSFER, true);

        // Execute request
        $oJSONResponse = curl_exec($oCurlConnection);
        if ($oJSONResponse === false) {
            $error = curl_error($oCurlConnection);
            echo "cURL Error: $error";
            exit;
        }

        $sHttpCode = curl_getinfo($oCurlConnection, CURLINFO_HTTP_CODE);
        if (is_numeric($sHttpCode)) {
            $iHttpCode = (int)$sHttpCode;   
        } else {
            $iHttpCode = -1;
            $sHttpCode = "-1";
        }

        curl_close($oCurlConnection);

        // Return Jira API response
        http_response_code($iHttpCode);

        $oJSONResponseData = json_decode($oJSONResponse, true); 

        $sJiraKey = $oJSONResponseData['key']; 

        if ($iHttpCode >= 200 && $iHttpCode < 300) {

            echo "<a href='http://www.ikawsoft.com/JiraForm/" . basename($_SERVER['PHP_SELF']) . "' style='color:white;'>Create Another Jira</a><br><br>";
            echo("<a href='https://{$sJiraDomain}/browse/{$sJiraKey}' style='color:white;' target='_blank'>View {$sJiraKey} In Jira</a>");

        } else {
            echo "Response Message: $sHttpCode : $oJSONResponse";
        }
        
          echo("</body>");
        echo("</html>");
        die();
    } 

?>


<body>

  <div class="form-container">
    <h2 style="color: #2e86c1;">Wolters Kluwer Create Jira</h2>

     <form id="frmJiraForm" action="" method="post">

      <div class="divJiraProject">
        <label for="jiratype">*Select Jira Project</label>
        <select id="cboJiraProject" name="cboJiraProject" required>
                <option value="">Select project...</option>
                <option value="WKHL">WK Health</option>
                <option value="VOP">Vendor Onboarding Portal</option>
        </select>
      </div>

      <div class="divFormDetails">
        <div class="divSummary">
            <label for="summary">*Summary</label>
            <input type="text" id="txtSummary" name="txtSummary" required>
        </div>

        <div class="divDescription">
            <label for="description">*Description</label>
            <textarea id="txtDescription" name="txtDescription" required></textarea>
        </div>

        <div class="divJiraType">
            <label for="jiratype">*Jira Type</label>
            <select id="cboJiraType" name="cboJiraType" required>


            </select>
        </div>

        <div class="divComponent">
            <label for="component">*Component</label>
            <select id="cboComponent" name="cboComponent" required>
                    
                
            </select>
        </div>
        <div class="divWKHealth">
            <div class="divBusinessRank">
                <label for="businessrank">Business Ranking</label>
                <select id="cboBusinessRank" name="cboBusinessRank">
                    <option value="">Select rank...</option>            
                    <option value="1">1</option>
                    <option value="2">2</option>
                    <option value="3">3</option>
                    <option value="4">4</option>
                    <option value="5">5</option>
                    <option value="6">6</option>
                    <option value="7">7</option>
                    <option value="8">8</option>
                    <option value="9">9</option>
                    <option value="10">10</option>
                    <option value="11">11</option>
                    <option value="12">12</option>
                    <option value="13">13</option>
                    <option value="14">14</option>
                    <option value="15">15</option>
                    <option value="16">16</option>
                    <option value="17">17</option>
                    <option value="18">18</option>
                    <option value="19">19</option>
                    <option value="20">20</option>
                    <option value="21">21</option>
                    <option value="22">22</option>
                    <option value="23">23</option>
                    <option value="24">24</option>
                    <option value="25">25</option>
                    <option value="26">26</option>
                    <option value="27">27</option>
                    <option value="28">28</option>
                    <option value="29">29</option>
                    <option value="30">30</option>

                </select>
                
            </div>

            <div class="divBusinessImpacted">
                <label for="businessimpacted">*Business Impacted</label>
                <select id="cboBusinessImpacted" name="cboBusinessImpacted" required>
                        <option value="">Select component...</option>
                        <option value="11319">HLP - Impacts book (PRO) and access (AMB) products</option>
                        <option value="11320">HLRP - Impacts all types of products</option>
                        <option value="11318">HR - Impacts journal (CIR) products</option>
                        <option value="11321">OPS - Impact to operations, not product specific</option>
                </select>
            </div>      
        </div>
        <button type="submit" style="float: right;">Create Jira</button>
    </div>
</br>
</br>
</form>
    
</body>
</html>

<script>


 // Define component options by project
    const componentsByProject = {
        "WKHL": [
            { value: "13790", text: "Advantage" },
            { value: "10385", text: "CDN" },
            { value: "10390", text: "EDI" },
            { value: "10392", text: "ESG" },
            { value: "10399", text: "eStore" },
            { value: "10396", text: "Firebrand" },
            { value: "10397", text: "IIB" },
            { value: "10400", text: "PTECom" },
            { value: "13791", text: "Salesforce" },
            { value: "10381", text: "Societies" },
            { value: "10408", text: "Vantage Point" }
        ],
        "VOP": [
            { value: "13129", text: "North America" },
            { value: "13669", text: "Health India" }
        ]
    };

    const JiraTypesByProject = {
        "WKHL": [
            { value: "10041", text: "Defect" },
            { value: "10042", text: "Change Request" },
            { value: "10040", text: "Data Issue" },
            { value: "10034", text: "Enhancement" },
            { value: "10032", text: "Service Request" }
        ],
        "VOP": [
            { value: "10025", text: "Change Request" },
            { value: "10027", text: "Bug" },
            { value: "10124", text: "Report" }
        ]
    };

//Default Form Load State
document.addEventListener("DOMContentLoaded", function() {
    const jiraProjectSelect = document.getElementById("cboJiraProject");
    const formDetailsDiv = document.querySelector(".divFormDetails");

    jiraProjectSelect.addEventListener("change", function() {
        if (jiraProjectSelect.value) {
            // Show when a non-empty option is chosen
            formDetailsDiv.style.display = "block";
        } else {
            // Hide again if user re-selects the blank option
            formDetailsDiv.style.display = "none";
        }
    });

   
});

    //Project Drop Down Changes
    document.addEventListener("DOMContentLoaded", function() {
        const jiraProjectSelect = document.getElementById("cboJiraProject");
        const oWKHealthDiv = document.querySelector(".divWKHealth");
        const oBusinessRank  = document.getElementById("cboBusinessRank");
        const oBusinessImpacted  = document.getElementById("cboBusinessImpacted");

        jiraProjectSelect.addEventListener("change", function() {
            
            if (jiraProjectSelect.value === "VOP") {
                // Hide the div
                oWKHealthDiv.style.display = "none";
                oBusinessRank.required = false;
                oBusinessImpacted.required = false;                                
            } else if (jiraProjectSelect.value === "WKHL") {
                // Show the div again
                oWKHealthDiv.style.display = "block";
                oBusinessRank.required = true;
                oBusinessImpacted.required = true;                
            } else {
                alert("No Project.");
            }

            loadComponents(jiraProjectSelect.value);
            loadJiraTypes(jiraProjectSelect.value);

        });
    });

    function loadComponents(projectKey) {
        const componentSelect = document.getElementById("cboComponent"); // <--
        
        // Clear existing options
        componentSelect.innerHTML = "";

        // Always add the default prompt option
        const defaultOption = document.createElement("option");
        defaultOption.value = "";
        defaultOption.textContent = "Select component...";
        componentSelect.appendChild(defaultOption);

        // Add project-specific options
        if (componentsByProject[projectKey]) {
            componentsByProject[projectKey].forEach(opt => {
                const option = document.createElement("option");
                option.value = opt.value;
                option.textContent = opt.text;
                componentSelect.appendChild(option);
            });
        }
    }

    function loadJiraTypes(projectKey) {
        const JiraTypeSelect = document.getElementById("cboJiraType"); // <--
        
        // Clear existing options
        JiraTypeSelect.innerHTML = "";

        // Always add the default prompt option
        const defaultOption = document.createElement("option");
        defaultOption.value = "";
        defaultOption.textContent = "Select component...";
        JiraTypeSelect.appendChild(defaultOption);

        // Add project-specific options
        if (JiraTypesByProject[projectKey]) {
            JiraTypesByProject[projectKey].forEach(opt => {
                const option = document.createElement("option");
                option.value = opt.value;
                option.textContent = opt.text;
                JiraTypeSelect.appendChild(option);
            });
        }
    }
</script>




