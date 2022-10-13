$(function () {
	
  /* ==========================================================================
     * set
   ==========================================================================*/
  //判斷是否存在
  $.fn.exist = function () {
    return this.length > 0;
  };

  /* ==========================================================================
     * header
   ==========================================================================*/
  $(".hamburger").click(function () {
    $(this).toggleClass("is-active");
    $(this).next('.nav').slideToggle(500, 'easeInOutCubic');
  });

  //
  $(window).resize(function () {
    if ($('.max-md-size').exist()) {
      $('#header').find('.nav').mCustomScrollbar({
        theme:"minimal-dark",
        axis: "y"
      });
    } else {
      $('#header').find('.nav').mCustomScrollbar("destroy");
    }
  }).resize();

  //
  function myMenu() {
    if ($('#header .menu > li').length) {
      var activeItem = $('#header .menu > li.is-active'),
          firstItem = $('#header .menu > li:first-child'),
          menuActive = $('#header').find('.menu-active'),
          activeL,
          activeW;      
      if(activeItem.length){
        activeL = activeItem.position().left;
        activeW = activeItem.innerWidth();
      }else{
        activeL = 0;
        activeW = firstItem.innerWidth();
      }
      $('#header .menu > li').each(function () {
        var childList = $(this).find('.child-list');
        menuActive.css({
          left: activeL,
          width: activeW
        });
        $(this).on('mouseenter.menu', function () {
          var el = $(this),
            x = el.position().left,
            w = el.innerWidth(),
            a = el.children('a'),
            tl = new TimelineLite();
          tl.to(menuActive, .1, {
              left: x,
              width: w,
              ease: Sine.easeOut
            })
          el.find('.child-list').stop().slideDown(300, 'easeOutSine');
        });
        $(this).on('mouseleave.menu', function () {
          childList.stop().hide(0);
        });
      });
      $(document).on('mouseleave.menu', function () {
        $('#header .menu > li').each(function () {
          tl = new TimelineLite();
          tl.to(menuActive, .3, {
              left: activeL,
              width: activeW,
              ease: Sine.easeInOut
            });
          $(this).find('.child-list').stop().hide(0);

        });
      });
      $('#header .menu').each(function () {
        $(this).on('mouseleave.menu', function () {
          var tl = new TimelineLite();
          tl.to(menuActive, .3, {
              left: activeL,
              width: activeW,
              ease: Sine.easeInOut
            });
        });
      });
    };
  }


  /* ==========================================================================
    * .banner parallax  //IE9 不使用設定
  ==========================================================================*/
  
  /*if (!$('html').hasClass('lt-ie9')){ 

	  $('#page-banner').each(function () {
	    $(this).find('.pic').addClass('layer').attr('data-depth', 0.01);
		  $('body').parallax({
		    scalarX: 45,
		    scalarY: 35,
		    frictionX: 0.1,
		    frictionY: 0.2,
		    calibrationDelay: 0,
		    supportDelay: 0
		  });
	  });
	  //console.log('ok')

  };*/



  /* ==========================================================================
     * 網頁防呆設定[內容不足高度時防破版] , resize
   ==========================================================================*/

  $(window).on('resize', function () {
    var footH = $('#footer').outerHeight();
    $('#main').css({
      paddingBottom: footH
    });
    $('#footer').css({
      marginTop: -footH
    });
    /*$(".pro55").each(function () {
      $(this).css("height", $(this).width());
    });
    $(".pro66").each(function () {
      $(this).css("height", $(this).width() * 0.66);
    });*/
  }).trigger('resize');


  /* ==========================================================================
     * pageTop 回到最上面
   ==========================================================================*/

  //
  $('.top').each(function() {
    $(this).on('click', function(e) {
        $('html,body').animate({
            scrollTop: $('body').offset().top
        }, 800);
        return false;
    });
  });


  /* ==========================================================================
     * 卷軸捲動時防呆
   ==========================================================================*/
  /*smooth scrollbar*/
  
  if (!$('html').hasClass('msie')){ //針對IE設定不使用
	  
	  $(document).on('mousewheel', function (event, delta) {
	    if (!$(event.target).is(':input')) {

	        var that = this,
	            duration = 800,
	            easing = 'easeOutCirc',
	            step = 80,
	            target = $('html, body'),
	            scrollHeight = $(document).height(),
	            scrollTop = that.last !== undefined ? that.last : $(window).scrollTop(),
	            viewportHeight = $(window).height(),
	            multiply = (event.deltaMode === 1) ? step : 1;


	        scrollTop -= delta * multiply * step;
	        scrollTop = Math.min((scrollHeight - viewportHeight), Math.max(0, scrollTop));
	        that.last = scrollTop;

	        target.stop().animate({
	            scrollTop: scrollTop
	        }, {
	            easing: easing,
	            duration: duration,
	            complete: function () {
	                delete that.last;
	            }
	        });

	        event.preventDefault();
	    }
	  });

  };

	$(".scrollbar-y").mCustomScrollbar({
	    theme: "light-thick",
	    axis: "y"
	});

	$(".table-wrapper").mCustomScrollbar({
	    theme: "light-thick",
	    axis: "x"
	});

  /* ==========================================================================
     * 圖片,連結禁止拖曳
   ==========================================================================*/

  $("img,a[href]").attr("draggable", "false");

  /* ==========================================================================
     * load完成後 卷軸美化 頁面顯示 捲動特效
   ==========================================================================*/
  $(".scrollbar").mCustomScrollbar({
    theme: "minimal-dark",
    // mouseWheel: { preventDefault: true }
  });
  
  $(".nav-sort .scrollbar-x").mCustomScrollbar({
    theme: "light-thick",
    axis: "x",
    callbacks: {
      onCreate: function () {
        var self = $(this);
        var el = this;
        self.prepend("<a class='nav-prev sprite sprite-banner-arrow-left'></a>");
        self.append("<a class='nav-next sprite sprite-banner-arrow'></a>");
        var $li = self.find('.inBox li');
        var prev = $('.nav-prev'),
          next = $('.nav-next');
        var x = 0;
        prev.addClass('disable');
        prev.click(function () {
          var pct = el.mcs.leftPct;
          if (pct != 0 && x > 0) {
            x--
          } else {
            x = x
          }
          self.mCustomScrollbar("scrollTo", $li.eq(x));
        });
        next.click(function () {
          var pct = el.mcs.leftPct;
          if (pct != 100 && x < $li.length) {
            x++
          } else {
            x = x
          }
          self.mCustomScrollbar("scrollTo", $li.eq(x));
        });
      },
      onScroll: function () {
        var prev = $('.nav-prev'),
          next = $('.nav-next');
        var pct = this.mcs.leftPct;
        if (pct == 0) {
          prev.addClass('disable');
        } else {
          prev.removeClass('disable');
        }
        if (pct == 100) {
          next.addClass('disable');
        } else {
          next.removeClass('disable');
        }

      }
    }
  });

  $('body').addClass("animated fadeIn");


});
