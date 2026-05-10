<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Site Under Maintenance</title>
</head>
<body style="margin:0; padding:0; background-color:#0f172a; font-family:Arial, sans-serif; color:#e5e7eb;">

  <div style="
    box-sizing:border-box;
    min-height:100vh;
    display:flex;
    flex-direction:column;
    justify-content:center;
    align-items:center;
    text-align:center;
    padding:24px;
  ">
    <div style="
      max-width:480px;
      width:100%;
      border-radius:12px;
      background:rgba(15,23,42,0.9);
      border:1px solid #1f2937;
      padding:32px 24px;
      box-shadow:0 20px 40px rgba(0,0,0,0.45);
    ">
      <div style="
        width:64px;
        height:64px;
        border-radius:999px;
        border:2px solid #fbbf24;
        display:flex;
        align-items:center;
        justify-content:center;
        margin:0 auto 20px auto;
        background:rgba(15,23,42,0.8);
      ">
        <div style="
          width:32px;
          height:32px;
          border-radius:999px;
          border:3px solid #fbbf24;
          border-top-color:transparent;
          animation:spin 1s linear infinite;
        "></div>
      </div>

      <h1 style="
        margin:0 0 12px 0;
        font-size:24px;
        letter-spacing:0.04em;
        text-transform:uppercase;
        color:#f9fafb;
      ">
        Undergoing Maintenance
      </h1>

      <p style="
        margin:0 0 16px 0;
        font-size:14px;
        line-height:1.6;
        color:#d1d5db;
      ">
        We’re performing some scheduled maintenance to improve your experience.
        The site will be back online shortly.
      </p>

      <p style="
        margin:0 0 4px 0;
        font-size:12px;
        text-transform:uppercase;
        letter-spacing:0.12em;
        color:#9ca3af;
      ">
        Status
      </p>
      <p style="
        margin:0 0 16px 0;
        font-size:13px;
        font-weight:bold;
        color:#fbbf24;
      ">
        In progress
      </p>

      <p style="
        margin:0;
        font-size:12px;
        color:#6b7280;
      ">
        If you reached this page in error, please try again in a few minutes.
      </p>
    </div>

    <div style="margin-top:16px; font-size:11px; color:#4b5563;">
      &copy; <span id="year" style="color:#9ca3af;"></span> Your Company. All rights reserved.
    </div>
  </div>

  <script>
    // Simple inline script just to keep the year current (optional)
    document.getElementById('year').textContent = new Date().getFullYear();
  </script>

  <!-- Inline keyframes since no external CSS -->
  <style>
    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  </style>
</body>
</html>