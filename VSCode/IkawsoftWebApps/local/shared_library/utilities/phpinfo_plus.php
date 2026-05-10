<!DOCTYPE html>
<html>
<head>
    <title>IIS + PHP Identity Diagnostics</title>
</head>
<body>

<h2>Windows / IIS Identity Diagnostics</h2>

<h3>whoami (actual Windows identity)</h3>
<pre>
    <?php
        exec('whoami', $whoami);
        print_r($whoami);
    ?>
</pre>

<h3>APP_POOL_ID</h3>
<pre>
    <?php
        echo $_SERVER['APP_POOL_ID'] ?? '(not set)';
    ?>
</pre>

<h3>USERNAME / USERDOMAIN (FastCGI environment)</h3>
<pre>
    <?php
        echo 'USERNAME: ' . ($_SERVER['USERNAME'] ?? '(none)') . "\n";
        echo 'USERDOMAIN: ' . ($_SERVER['USERDOMAIN'] ?? '(none)') . "\n";
    ?>
</pre>

<h3>get_current_user()</h3>
<pre>
    <?php
        echo get_current_user();
    ?>
</pre>

<hr>

<h2>phpinfo()</h2>

    <?php phpinfo(); ?>

</body>
</html>
