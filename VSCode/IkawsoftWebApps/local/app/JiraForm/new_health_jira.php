<!DOCTYPE html>
<html>
<head>
    <title>WKHL Create Jira</title>
</head>
<body>


<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Wolters Kluwer Health (WKHL) Create Jira</title>
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
  </style>
</head>
<body>

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
        $sSummary = (string)$_POST["txtSummary"];
        $sDescription = (string)$_POST["txtDescription"];
        $iJiraType = $_POST["cboJiraType"];
        $iComponent = $_POST["cboComponent"];
        $iBusinessRank = (int)$_POST["cboBusinessRank"];
        $sBusinessImpacted = (string)$_POST["cboBusinessImpacted"];


        // Jira API endpoint
        

        // Prepare payload
          $oNewJiraData = [
              'fields' => [
                  'project' => ['key' => 'WKHL'],
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
                  'customfield_10210' => $iBusinessRank,
                  'customfield_10223' => ['id' => $sBusinessImpacted],
              ]
          ];


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

            echo("<a href='http://www.ikawsoft.com/JiraForm/index.php' style='color:white;'>Create Another Jira</a><br><br>");
            echo("<a href='https://{$sJiraDomain}/browse/{$sJiraKey}' style='color:white;' target='_blank'>View {$sJiraKey} In Jira</a>");

        } else {
            echo "Response Message: $sHttpCode : $oJSONResponse";
        }
        
          echo("</body>");
        echo("</html>");
        die();
    } 

?>


<!DOCTYPE html>
<html>
<head>
    <title>WKHL Create Jira</title>
</head>
<body>


<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Wolters Kluwer Health (WKHL) Create Jira</title>
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
  </style>
</head>
<body>
  <div class="form-container">
    <h2 style="color: #2e86c1;">Wolters Kluwer Health (WKHL) Create Jira</h2>

        <form id="frmJiraForm" action="" method="post">

      <div class="form-group">
        <label for="summary">*Summary</label>
        <input type="text" id="txtSummary" name="txtSummary" required>
      </div>

      <div class="form-group">
        <label for="description">*Description</label>
        <textarea id="txtDescription" name="txtDescription" required></textarea>
      </div>

      <div class="form-group">
        <label for="jiratype">*Jira Type</label>
        <select id="cboJiraType" name="cboJiraType" required>
                <option value="">Select type...</option>
                <option value="10041">Defect</option>
                <option value="10042">Change Request</option>
                <option value="10040">Data Issue</option>
                <option value="10034">Enhancement</option>
                <option value="10032">Service Request</option>

        </select>
      </div>

      <div class="form-group">
        <label for="component">*Component</label>
        <select id="cboComponent" name="cboComponent" required>
                <option value="">Select component...</option>
                <option value="13790">Advantage</option>
                <option value="10385">CDN</option>
                <option value="10390">EDI</option>
                <option value="10392">ESG</option>
                <option value="10399">eStore</option>
                <option value="10396">Firebrand</option>
                <option value="10397">IIB</option>
                <option value="10400">PTECom</option>
                <option value="13791">Salesforce</option>
                <option value="10381">Societies</option>
                <option value="10408">Vantage Point</option>
            
        </select>
      </div>

      <div class="form-group">
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

      <div class="form-group">
        <label for="businessimpacted">*Business Impacted</label>
        <select id="cboBusinessImpacted" name="cboBusinessImpacted" required>
                <option value="">Select component...</option>
                <option value="11319">HLP - Impacts book (PRO) and access (AMB) products</option>
                <option value="11320">HLRP - Impacts all types of products</option>
                <option value="11318">HR - Impacts journal (CIR) products</option>
                <option value="11321">OPS - Impact to operations, not product specific</option>
        </select>
      </div>      
      <button type="submit" style="float: right;">Create Jira</button>
</br>
</br>
</form>
    
</body>
</html>




