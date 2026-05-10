function cycleAnchorClass() {
  const anchors = document.querySelectorAll("#main header h1 a"); // Select all anchor tags inside h1
  let index = 0;
  const className = "highlight"; // Change this to your desired class

  function applyClass() {
    if (anchors.length === 0) return;

    // Remove the class from all anchors
    anchors.forEach((a) => a.classList.remove(className));

    // Add the class to the current anchor
    anchors[index].classList.add(className);

    // Remove class after 2 seconds
    setTimeout(() => {
      if (index == anchors.length - 1) {
        index = 0;
      } else {
        index++;
      }
    }, 2000);
  }

  // Run initially
  applyClass();

  // Repeat every 2 seconds
  setInterval(applyClass, 2000);
}

// Call the function
cycleAnchorClass();

document.addEventListener("DOMContentLoaded", function () {
  var backToTopButton = document.getElementById("back-to-top");

  if (backToTopButton) {
    var scrollTrigger = 100; // px

    function backToTop() {
      if (window.scrollY > scrollTrigger) {
        backToTopButton.classList.add("show");
      } else {
        backToTopButton.classList.remove("show");
      }
    }

    // Run initially
    backToTop();

    // Listen for scroll events
    window.addEventListener("scroll", backToTop);

    // Smooth scroll to top on click
    backToTopButton.addEventListener("click", function (e) {
      e.preventDefault();
      window.scrollTo({ top: 0, behavior: "smooth" });
    });
  }

  document.getElementById("year").textContent = new Date().getFullYear();

  // Lightbox
  document.querySelectorAll(".lightbox_trigger").forEach(function (trigger) {
    trigger.addEventListener("click", function (e) {
      e.preventDefault();

      // Get clicked link href
      var imageHref = this.getAttribute("href");

      // Check if lightbox already exists
      var lightbox = document.getElementById("lightbox");

      if (lightbox) {
        // Update image src
        document.getElementById("lightBoxContent").innerHTML =
          '<img src="' + imageHref + '" />';
        lightbox.style.display = "block";
      } else {
        // Create lightbox HTML and insert it into the body
        lightbox = document.createElement("div");
        lightbox.id = "lightbox";
        lightbox.innerHTML = `
                <p id="lightboxClose"><i id="lightboxCloseIcon" class="fa fa-times" aria-hidden="true"></i></p>
                <div id="lightBoxContent">
                    <img src="${imageHref}" />
                </div>
            `;
        document.body.appendChild(lightbox);
      }
    });
  });

  // Click anywhere on the lightbox to close it
  document.body.addEventListener("click", function (e) {
    if (
      e.target.id === "lightbox" ||
      e.target.id === "lightboxClose" ||
      e.target.id === "lightboxCloseIcon"
    ) {
      document.querySelector("#lightbox").style.display = "none";
    }
  });
});

window.addEventListener("load", function () {
  document.querySelector("#loader-wrapper").style.display = "none";
});

(function ($) {
  var $window = $(window),
    $body = $("body");

  // Breakpoints.
  breakpoints({
    xlarge: ["1281px", "1680px"],
    large: ["981px", "1280px"],
    medium: ["737px", "980px"],
    small: ["481px", "736px"],
    xsmall: ["361px", "480px"],
    xxsmall: [null, "360px"]
  });

  // Play initial animations on page load.
  $window.on("load", function () {
    window.setTimeout(function () {
      $body.removeClass("is-preload");
    }, 100);
  });

  // Touch?
  if (browser.mobile) $body.addClass("is-touch");

  // Forms.
  var $form = $("form");

  // Auto-resizing textareas.
  $form.find("textarea").each(function () {
    var $this = $(this),
      $wrapper = $('<div class="textarea-wrapper"></div>'),
      $submits = $this.find('input[type="submit"]');

    $this
      .wrap($wrapper)
      .attr("rows", 1)
      .css("overflow", "hidden")
      .css("resize", "none")
      .on("keydown", function (event) {
        if (event.keyCode == 13 && event.ctrlKey) {
          event.preventDefault();
          event.stopPropagation();

          $(this).blur();
        }
      })
      .on("blur focus", function () {
        $this.val($.trim($this.val()));
      })
      .on("input blur focus --init", function () {
        $wrapper.css("height", $this.height());

        $this
          .css("height", "auto")
          .css("height", $this.prop("scrollHeight") + "px");
      })
      .on("keyup", function (event) {
        if (event.keyCode == 9) $this.select();
      })
      .triggerHandler("--init");

    // Fix.
    if (browser.name == "ie" || browser.mobile)
      $this.css("max-height", "10em").css("overflow-y", "auto");
  });

  // Menu.
  var $menu = $("#menu");

  $menu.wrapInner('<div class="inner"></div>');

  $menu._locked = false;

  $menu._lock = function () {
    if ($menu._locked) return false;

    $menu._locked = true;

    window.setTimeout(function () {
      $menu._locked = false;
    }, 350);

    return true;
  };

  $menu._show = function () {
    if ($menu._lock()) $body.addClass("is-menu-visible");
  };

  $menu._hide = function () {
    if ($menu._lock()) $body.removeClass("is-menu-visible");
  };

  $menu._toggle = function () {
    if ($menu._lock()) $body.toggleClass("is-menu-visible");
  };

  $menu
    .appendTo($body)
    .on("click", function (event) {
      event.stopPropagation();
    })
    .on("click", "a", function (event) {
      var href = $(this).attr("href");

      event.preventDefault();
      event.stopPropagation();

      // Hide.
      $menu._hide();

      // Redirect.
      if (href == "#menu") return;

      window.setTimeout(function () {
        window.location.href = href;
      }, 350);
    })
    .append('<a class="close" href="#menu">Close</a>');

  $body
    .on("click", 'a[href="#menu"]', function (event) {
      event.stopPropagation();
      event.preventDefault();

      // Toggle.
      $menu._toggle();
    })
    .on("click", function (event) {
      // Hide.
      $menu._hide();
    })
    .on("keydown", function (event) {
      // Hide on escape.
      if (event.keyCode == 27) $menu._hide();
    });
})(jQuery);
