<?php

        try {

            $sLoginPageURL = $_GET['LoginPageURL'] ?? "";
            $sSessionName = $_GET['SessionName'] ?? "";

            if (strlen($sSessionName) > 0) {
                session_name($sSessionName);
                session_start();

                if (session_status() !== PHP_SESSION_NONE) {

                    // Unset all session variables
                    $_SESSION = array();

                    // If you want to kill the session, also delete the session cookie
                    if (ini_get("session.use_cookies")) {
                        $params = session_get_cookie_params();
                        setcookie(session_name(), '', time() - 42000,
                            $params["path"], $params["domain"],
                            $params["secure"], $params["httponly"]
                        );
                    }

                    // Finally, destroy the session
                    session_destroy();
                }
            } else {
                throw new Exception("Unable to end session without a Session Name.");
            }

            if (strlen($sLoginPageURL) > 0) {
                header("Location: " . $sLoginPageURL);
                exit;
            } else {
                throw new Exception("Login page URL required.");
            }

        } catch (Throwable $oError) {
            throw $oError;
        }

?>