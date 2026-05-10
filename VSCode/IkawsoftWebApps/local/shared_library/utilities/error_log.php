<!DOCTYPE html>
<?php

    if (!defined('URL_SEPARATOR')) {
        define('URL_SEPARATOR', '/');
    }


    require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

    $sFileName = "";
    $sAppFolder = "";
    $sAppID = -1;
    $sFilePath = "";
    $sFileURL = "";
    $sServerRootURL = "";
    $sQueryString = "";
    $oAppID = null;

    $sAppFolder = $_GET['ApplicationFolder'] ?? '';
    $sAppID = $_GET['AppID'] ?? '';
    $iAppID = (int)$sAppID;
    $eAppID = ApplicationID::tryFrom($iAppID);

    $sAppFolder = urldecode($sAppFolder);
    $sAppID = urldecode($sAppID);
    
    $sFileName = 'app_error.txt';
    $sFilePath = $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . mGetApplicationEnvironment($_SERVER['REQUEST_URI']) . DIRECTORY_SEPARATOR . "app" . DIRECTORY_SEPARATOR . $sAppFolder . DIRECTORY_SEPARATOR . $sFileName;

    $sFileURL = mGetRootPath($eAppID, enuPathRootType::URL, enuPathType::ApplicationFolder, enuFolderType::NoSubFolder) . $sFileName;

    
    // Handle file clearing
    if ($_SERVER['REQUEST_METHOD'] === 'GET' && isset($_GET['clear_file'])) {
        $sQueryString = "?ApplicationFolder=" . urlencode($sAppFolder) . "&AppID=" . urlencode($sAppID);
        file_put_contents($sFilePath, ''); // Overwrite with empty string
        header("Location: " . $_SERVER['PHP_SELF'] . $sQueryString ); // Refresh to show updated content
        exit;
    }

    // Read the file contents
    $content = '';
    if (file_exists($sFilePath)) {
        $content = htmlspecialchars(file_get_contents($sFilePath));
    } else {
        $content = 'Error Log page cannot find log file. ' . $sFilePath;
    }

?>


<html>
<head>
    <title>View Log</title>
    <style>
        html, body {
            height: 100%;
            margin: 0;
            display: flex;
            flex-direction: column;
        }
        h2 {
            margin: 1em;
        }
        form {
            flex: 1;
            display: flex;
            flex-direction: column;
            padding: 1em;
        }
        textarea {
            flex: 1;
            resize: vertical;
            overflow: auto;
            width: 100%;
            height: 80vh;
            box-sizing: border-box;
            margin-bottom: 1em;
        }
        button {
            align-self: flex-start;
        }
    </style>
<style>
    #output table {
        border-collapse: collapse;
        width: 100%;
    }
    #output table td, 
    #output table th {
        border: 1px solid #ccc;
        padding: 4px 8px;
        text-align: left;
    }
    #output table th {
        background: #f0f0f0;
        font-weight: bold;
    }

    #output table td {
    background: #fff;
    }

    #output table td[contenteditable] {
        outline: 1px solid #ccc;
    }

</style>
    <script src="https://cdn.sheetjs.com/xlsx-latest/package/dist/xlsx.full.min.js"></script>
    <script src="https://unpkg.com/canvas-datagrid/dist/canvas-datagrid.js"></script>


</head>
<body>

    <h2>Contents of <?= htmlspecialchars($sFileURL) ?></h2>
    
    
    <?php
        $sFormAction = htmlspecialchars($_SERVER['PHP_SELF']);
        
    ?>
    
    <form method="get" action="<?php echo $sFormAction; ?>">
        <input type="hidden" name="ApplicationFolder" value="<?php echo urlencode($sAppFolder); ?>">
	    <input type="hidden" name="AppID" value="<?php echo urlencode($sAppID); ?>">
        <div id="output"></div>
        <br>
        <button type="button" onclick="mDownloadGrid()">Download To Excel</button>
        <!-- <button type="submit" name="clear_file" value="1"
                onclick="return confirm('Are you sure you want to clear the contents of the file?');">
            Clear File
        </button> -->


    </form>'


<script>
    async function mLoadFileIntoSheetJS(sFilePath) {
        try {

            const response = await fetch(sFilePath);

            if (!response.ok) {
                throw new Error("Failed to load log file. HTTP " + response.status);
            }

            const csvText = await response.text();

            // Parse pipe-delimited file
            const workbook = XLSX.read(csvText, {
                type: "string",
                FS: "|"   // <— Pipe Delimited
            });

            const firstSheet = workbook.SheetNames[0];
            const worksheet = workbook.Sheets[firstSheet];

            const data = XLSX.utils.sheet_to_json(worksheet, { header: 1 });

            // Extract header row
            const headers = data[0];

            // Extract data rows
            const rows = data.slice(1);

            // Build objects so Canvas‑Datagrid uses headers as column names
            const formatted = rows.map(row => {
                const obj = {};
                headers.forEach((h, i) => obj[h] = row[i]);
                return obj;
            });

            // Sort by date descending (newest first)
            formatted.sort((a, b) => {
                const da = new Date(a["Timestamp"]);
                const db = new Date(b["Timestamp"]);
                return db - da; // newest first
            });

            const grid = canvasDatagrid({
                parentNode: document.getElementById("output"),
                data: formatted
            });

            document.getElementById("output").dataGrid = grid;

        } catch (oError) {
            alert(oError.message);
        }
    }


    mLoadFileIntoSheetJS(<?php echo json_encode($sFileURL); ?>);

    function mDownloadGrid() {
        try {
            const container = document.getElementById("output");

            // Canvas‑Datagrid attaches itself as .dataGrid
            const grid = container.dataGrid;

            if (!grid) {
                alert("Grid not initialized");
                return;
            }

            const data = grid.data; // current grid contents

            const worksheet = XLSX.utils.json_to_sheet(data);
            const workbook = XLSX.utils.book_new();
            XLSX.utils.book_append_sheet(workbook, worksheet, "LogData");

            XLSX.writeFile(workbook, "log_export.xlsx");
        } catch (oError) {
            alert(oError.message);
        }        
    }



</script>
    
    
</body>
</html>




