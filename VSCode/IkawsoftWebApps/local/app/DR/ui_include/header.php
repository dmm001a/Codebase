<?php
    $colYear = [
        'Y2025' => 5,
        'Y2026' => 10,
        'Y2027' => 15,
        'Y2028' => 20,
        'Y2029' => 25,
        'Y2030' => 30,    
    ];

    $sLogoutLink = mGetRootPath(ApplicationID::DR, enuPathRootType::URL, enuPathType::SharedLibraryFolder, enuFolderType::lib) . 'logout.php?SessionName=' . session_name();
    $sLogoutLink .= "&LoginPageURL=" . urlencode(
        mGetRootPath(
            ApplicationID::DR,
            enuPathRootType::URL,
            enuPathType::ApplicationFolder,
            enuFolderType::NoSubFolder
        ) . "login.php"
    );
?>

<nav class="navbar navbar-expand-lg navbar-dark navbar-custom py-3">
    <div class="container-fluid">

        <a class="navbar-brand d-flex align-items-center gap-2" href="#">
            <div class="brand-icon" style="margin-right: 5px;">
                <i class="bi bi-shield-check"></i>
            </div>
            <div class="d-flex flex-column">
                <span class="fw-bold lh-1">Disaster Recovery Tracker</span>
                <span class="fs-7 opacity-75 fw-light">From Test Design to Test Execution</span>
            </div>
        </a>

        <div id="LogoutButtonDiv" style="margin-left:20px;">
            <a href="<?php echo($sLogoutLink); ?>" class="btn btn-outline-light btn-sm d-flex align-items-center gap-2 rounded-pill px-3 glass-btn">
                <i class="bi bi-box-arrow-right"></i> Logout
            </a>
        </div>

        <button class="navbar-toggler border-0" type="button" data-bs-toggle="collapse" data-bs-target="#navbarContent">
            <span class="navbar-toggler-icon"></span>
        </button>

        <div class="collapse navbar-collapse" id="navbarContent">

            <!-- WRAPPER: filters + feature request -->
            <div class="d-flex align-items-center ms-auto gap-5">

                <!-- Filters -->
                <div class="d-flex align-items-center gap-3 my-3 my-lg-0 nav-filters">

                    <div class="nav-item-group">
                        <label class="nav-label px-2">Platform</label>
                        <?php
                            try {
                                $oDropDown = new cControl();
                                $oDropDown->mOutputDropDown(
                                    ApplicationID::DR->value,
                                    "cboPlatform",
                                    "cboPlatform",
                                    "",
                                    "",
                                    "",
                                    "",
                                    "Platform",
                                    $_SESSION["SelectedPlatform"],
                                    "",
                                    "form-select form-select-sm header-select"
                                );
                                $oDropDown = null;
                            } catch (Throwable $oError) {
                                throw $oError;
                            }
                        ?>
                    </div>

                    <div class="nav-divider"></div>

                    <div class="nav-item-group">
                        <label class="nav-label px-2">Year</label>
                        <?php
                            try {
                                if ($_SESSION["SelectedYear"] === "" || $_SESSION["SelectedYear"] === -1) {
                                    $sYearID = "Y" . date("Y");
                                    $_SESSION["SelectedYear"] = $colYear[$sYearID];
                                }
                                $oDropDown = new cControl();
                                $oDropDown->mOutputDropDown(
                                    ApplicationID::DR->value,
                                    "cboYear",
                                    "cboYear",
                                    "",
                                    "",
                                    "",
                                    "",
                                    "Year",
                                    $_SESSION["SelectedYear"],
                                    "",
                                    "form-select form-select-sm header-select"
                                );
                                $oDropDown = null;
                            } catch (Throwable $oError) {
                                throw $oError;
                            }
                        ?>
                    </div>

                    <div class="nav-divider"></div>

                    <div class="nav-item-group">
                        <label class="nav-label px-2">Page</label>
                        <?php
                            try {
                                $oDropDown = new cControl();
                                $oDropDown->mOutputDropDown(
                                    ApplicationID::DR->value,
                                    "cboPage",
                                    "cboPage",
                                    "",
                                    "",
                                    "",
                                    "",
                                    "Page",
                                    $_SESSION["SelectedPage"],
                                    "",
                                    "form-select form-select-sm header-select"
                                );
                                $oDropDown = null;
                            } catch (Throwable $oError) {
                                throw $oError;
                            }
                        ?>
                    </div>

                </div> <!-- END nav-filters -->

              <!-- Feature Request block -->
              <div class="d-flex flex-column ms-2">
                  <a href="#" class="text-white fw-light" onclick="mCopyToClipboard('gbs-es-disaster-recovery@wolterskluwer.com', 'Please send a description of your feature or fix request to this e-mail address.');">
                      Feature/Fix Request
                  </a>
              </div>


            </div> <!-- END wrapper -->

        </div>
    </div>
</nav>
