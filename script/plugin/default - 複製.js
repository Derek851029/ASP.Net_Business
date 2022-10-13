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

  $(window).resize(function () {
    if ($('.min-md-size').exist()) {
      $('#header').find('.nav .child2').mCustomScrollbar({
        theme:"minimal-dark",
        axis: "y"
      });
    } else {
      $('#header').find('.nav .child2').mCustomScrollbar("destroy");
    }
  }).resize();

  //
  $(window).resize(function () {
    if ($('.min-md-size').exist()) {
      $('#header').find('.nav .child2 .sub').mCustomScrollbar({
        theme:"minimal-dark",
        axis: "y"
      });
    } else {
      $('#header').find('.nav .child2 .sub').mCustomScrollbar("destroy");
    }
  }).resize();

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

  function mobileMenu(){
   // $('#header .menu > li > a').each(function(){
      $('#header .menu  a').on('click',function(e){
        var parentli = $(this).parent('li');
        var hasChild = parentli.hasClass('has-child');
        var $child = $(this).next('.sub');
        var $siblings = parentli.siblings('.has-child');
        
        if(hasChild == true){

          e.preventDefault();
          parentli.toggleClass('active');

          $child.stop().slideToggle(500,'easeOutCubic');
          $siblings.removeClass('active')
          $siblings.find('.sub').slideUp(0,'easeOutCubic');
        }
      })

      //產品比較表卷軸
      $(".af-scrollbar").mCustomScrollbar({
	    theme: "light-thick",
	    axis: "y"
	  });
  }

  myMenu();

  var h = 0;

  $(window).on('resize', function() {
    var w1024 = Modernizr.mq('(max-width: 1024px)');
      if (w1024 && h == 0) {
          $('#header .menu > li').off('mouseenter.menu mouseleave.menu');
          $('#header .menu').off('mouseleave.menu');
          $(document).off('mouseleave.menu');
          mobileMenu();
          h = h + 1;
          return h;
      } else if (!w1024 && h == 1) {
          $('#header .menu > li > a').off('click');
          myMenu();
          h = 0;
          return h;
      }
  }).trigger('resize');

  

  /*文自動畫/特效*/
  var splitText = {
    splitAni: function (text, trigger, animation, offset) {
      if (text.html().length) {
        var oldHtml = text.html().split('');
        var newHtml = [];
        for (var i = 0; i < oldHtml.length; i++) {
          if (oldHtml[i] == " ") {
            oldHtml[i] = "&nbsp;"
          }
          newHtml.push('<span>' + oldHtml[i] + '</span>');
        }
        text.html(newHtml);
        if (text.find('span').length) {
          text.css('opacity', 1);
          var span = text.find('span');
          var controller = new ScrollMagic.Controller();
          switch (animation) {
          case "fadeInUp":
            span.css({
              transform: 'translateY(100px)'
            });
            for (var i = 0; i < span.length; i++) {
              new ScrollMagic.Scene({
                  triggerElement: trigger || span[i],
                  offset: offset || 0
                })
                .setTween(span[i], .5, {
                  opacity: 1,
                  transform: 'translateY(0)',
                  delay: (i * 0.1).toFixed(1),
                  ease: Sine.easeOut
                })
                .addTo(controller);
            }
            break;
          case "rotateX":
            span.css({
              transform: 'rotateX(90deg)'
            });
            for (var i = 0; i < span.length; i++) {
              new ScrollMagic.Scene({
                  triggerElement: trigger || span[i],
                  offset: offset || 0
                })
                .setTween(span[i], .5, {
                  opacity: 1,
                  transform: 'rotateX(0deg)',
                  delay: (i * 0.1).toFixed(1),
                  ease: Sine.easeOut
                })
                .addTo(controller);
            }
            break;
          default:
            for (var i = 0; i < span.length; i++) {
              new ScrollMagic.Scene({
                  triggerElement: trigger || span[i],
                  offset: offset || 0
                })
                .setTween(span[i], .5, {
                  opacity: 1,
                  delay: (i * 0.1).toFixed(1),
                  ease: Sine.easeOut
                })
                .addTo(controller);
            }
            break;

          }
        }
      }
    }
  };
  
  $('#banner .splitText-fadeInUp').each(function () {
    splitText.splitAni($(this), $('#banner'), 'fadeInUp');
  });
  $('#banner .splitText-rotateX').each(function () {
    splitText.splitAni($(this), $('#banner'), 'rotateX');
  });
  $('#page-banner .splitText').each(function () {
    splitText.splitAni($(this), $('#page-banner'), 'rotateX');
  });

  /* ==========================================================================
    * magic scroll
  ==========================================================================*/
  
  var controller = new ScrollMagic.Controller();
  var magicFadeIn = document.querySelectorAll(".magic-fadeIn");
  for (var i = 0; i < magicFadeIn.length; i++) {
    new ScrollMagic.Scene({
        triggerElement: magicFadeIn[i],
        offset: -200
      })
      .setTween(magicFadeIn[i], 1, {
        opacity: 1,
        ease: Sine.easeOut
      })
      .addTo(controller);
  };

  var magicFadeInUp = document.querySelectorAll(".magic-fadeInUp");
  for (var i = 0; i < magicFadeInUp.length; i++) {
    new ScrollMagic.Scene({
        triggerElement: magicFadeInUp[i],
        offset: -200
      })
      .setTween(magicFadeInUp[i], .5, {
        transform: "translateY(0)",
        opacity: 1,
        ease: Sine.easeOut
      })
      .addTo(controller);
  };

  var magicFadeInLeft = document.querySelectorAll(".magic-fadeInLeft");
  for (var i = 0; i < magicFadeInLeft.length; i++) {
    new ScrollMagic.Scene({
        triggerElement: magicFadeInLeft[i],
        offset: -200
      })
      .setTween(magicFadeInLeft[i], .5, {
        transform: "translateX(0)",
        opacity: 1,
        ease: Sine.easeOut
      })
      .addTo(controller);
  };

  var magicFadeInRight = document.querySelectorAll(".magic-fadeInRight");
  for (var i = 0; i < magicFadeInRight.length; i++) {
    new ScrollMagic.Scene({
        triggerElement: magicFadeInRight[i],
        offset: -200
      })
      .setTween(magicFadeInRight[i], .5, {
        transform: "translateX(0)",
        opacity: 1,
        ease: Sine.easeOut
      })
      .addTo(controller);
  };


  /* ==========================================================================
    * eco [mouse]
  ==========================================================================*/
 
  $('.color-lake').each(function() {
      //往下滾動
      $('#eco .mouse').click(function(){
		$('html,body').animate({scrollTop:$('.reuse').offset().top - 50 }, 800);
      });
  });

  /* ==========================================================================
    * home [mouse]
  ==========================================================================*/
 
  $('.home').each(function() {
      //往下滾動
      $('.home .mouse').click(function(){
		$('html,body').animate({scrollTop:$('.quickly-gates-01').offset().top - 150 }, 800);
      });
  });

  /* ==========================================================================
    * home [news-slider]
  ==========================================================================*/
  
  $('#news-slider').each(function () {
    $(this).find('.slider').slick({
      dots: false,
      arrows: true,
      slidesToShow: 3,
      slidesToScroll: 1,
      autoplay: true,
      speed: 800,
      cssEase: 'ease',
      easing:'easeInOutCirc',
      autoplaySpeed: 5000,
      //centerMode: true,/*左右半畫面，必設定*/
      //centerPadding: '25%',/*左右半畫面，必設定*/
      responsive: [
        {
      breakpoint: 820,
      settings: {
      slidesToShow: 2,
      slidesToScroll: 2
      //centerPadding: '0'
        }
        },{
      breakpoint: 580,
      settings: {
      slidesToShow: 1,
      slidesToScroll: 1
      //centerPadding: '0'
        }
      }
      ]
    });
  });

  /* ==========================================================================
    * about [history-slider]
  ==========================================================================*/
  
  $('#history-slider').each(function () {
    $(this).find('.slider').slick({
      dots: false,
      arrows: true,
      slidesToShow: 5,
      slidesToScroll: 1,
      autoplay: true,
      speed: 800,
      cssEase: 'ease',
      easing:'easeInOutCirc',
      autoplaySpeed: 5000,
      responsive: [
      	{
	      breakpoint: 1366,
	      settings: {
		      slidesToShow: 4
      		}
      	},{
	      breakpoint: 1024,
	      settings: {
		      slidesToShow: 3
      		}
      	},{
	      breakpoint: 768,
	      settings: {
		      slidesToShow: 2,
		      slidesToScroll: 2
      		}
      	},{
	      breakpoint: 580,
	      settings: {
		      slidesToShow: 1,
		      slidesToScroll: 1
	        }
      	}
      ]
    });
  });

  /* ==========================================================================
    * recruit [pic-slider]
  ==========================================================================*/
	$("#recruit").each(function() {
		$(this).find('.pic-slider').slick({
			dots: false,
			arrows: true,
			autoplay: true,
			autoplaySpeed: 6000,
			infinite: true,
			speed: 1300,
			slidesToShow: 4,
			slidesToScroll: 1,
			variableWidth: true,
			pauseOnHover: false,
			easing: 'easeInOutExpo',
			responsive: [
				{
				  breakpoint: 991,
				  settings: {
					slidesToShow: 2,
				  }
				},{
				  breakpoint: 480,
				  settings: {
					slidesToShow: 1,
				  }
				}
			]
		});
	});

  	/* ==========================================================================
                * fullPage
    ==========================================================================*/

	$("#eco").each(function() {
	    $(this).find(".fullpage").fullpage({
			responsiveWidth: 1199,
			responsiveHeight: 970,
			navigation: true,
			navigationPosition: 'right',
			navigationTooltips: [
				/*'banner', // banner*/
				'回收方式', // paper
				'資源再利用', // reuse
				'回收流程', // process
				'紙容器Q&A'/*, // qa
				'تاجر محدد', // dealer
				'info', // info*/
			],
			anchors: [
				/*'banner',*/
				'paper',
				'reuse',
				'process',
				'qa'/*,
				'dealer',
				'info'*/
			],
			//scrollingSpeed: 800,
			scrollBar:true
		});
	});

	/* ==========================================================================
    * eco [QA]
  ==========================================================================*/
  
  $('#eco').each(function() {
    $('.qaMenuInco').click(function() {
            $('.qaMenuList').toggle();
     }); 
	 
	 accordionOpen();
	//上下開合效果------------------------------------------
	function accordionOpen(){
	  $('.qa-box').each(function() {
	    $('.qa-box .qa-title a').append('<span></span>').click(function(){
			$(this).parent().toggleClass('actived').next().slideToggle();
			$('.qa-title').not($(this).parent()).removeClass('actived');
			$('.qa-info').not($(this).parent().next()).slideUp();
	    });
	  });
	}
 });



  /* ==========================================================================
			[dolls-what] 點擊彈跳視窗
	==========================================================================*/
  $('.dolls-milk').each(function() {
	$(".dolls-close, .milk-zoom").click(function() {
		$(".dolls-what").toggleClass("active");
	});
  });	

  /* ==========================================================================
			[winBox] 彈跳視窗
	==========================================================================*/
  $(".open-box").click(function () {
	$(".winBox").addClass('box-active');
  });
	
  $(".close").click(function () {
	$(".winBox").removeClass('box-active')
  });

  /* ==========================================================================
    * .fancybox-video
  ==========================================================================*/

  /*$(".fancybox-video").fancybox({
    type: 'iframe',
    openEffect: 'fade',
    closeEffect: 'fade',
    padding: 0,
    aspectRatio: true,
    helpers: {
      overlay: {
        locked: false
      }
    }
  });*/

  /* ==========================================================================
     * fancybox
   ==========================================================================*/
   /*$(".modal").fancybox({
    width: '100%',
    height: '90%',
    openEffect: 'fade',
    closeEffect: 'fade',
    padding: 0,
    afterLoad: function () {
      $('.fancybox-skin').addClass('modal-fancybox')
    },
	helpers: {
      overlay: {
        locked: false
      }
    }
  });*/

  /* ==========================================================================
    * product [pro-slider]
  ==========================================================================*/
  $('#pro-slider').each(function() {
            
	$('.slider-for').slick({
		autoplay: true,
		slidesToShow: 1,
		slidesToScroll: 1,
		arrows: true,
		fade: false,
		slide: '.list',
		cssEase: 'cubic-bezier(0.645, 0.045, 0.355, 1.000)',
		asNavFor: '.slider-nav'
	});

	$('.slider-nav').slick({
		autoplay: true,
		slidesToShow: 9,
		slidesToScroll: 1,
		arrows: false,
		slide: '.team',
		dots: false,
		centerMode: false,
		focusOnSelect: false,
		asNavFor: '.slider-for',
		responsive: [
			{
		      breakpoint: 1280,
				settings: {
					slidesToShow: 7,
				}
	        },
	        {
		        breakpoint: 992,
		        settings: {
			        slidesToShow: 5,
		      	}
	        },
	        {
		        breakpoint: 680,
		        settings: {
			        slidesToShow: 4,
		      	}
	        },
	        {
		        breakpoint: 580,
		        settings: {
			        slidesToShow: 3,
		      	}
	        },
	        {
		        breakpoint: 480,
		        settings: {
			        slidesToShow: 2,
		      	}
	        }
        ]
	});
  });

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
    * .widget-len
  ==========================================================================*/
  
  $('.lang-in').click(function() {
	$('.widget-lan').children('.level-2').stop().slideToggle(500,'easeOutCubic');
  });

  /* ==========================================================================
    * 下拉選單套件
  ==========================================================================*/
  
  $(".normal_select").dropkick({
  	mobile: true
  });

  /* ==========================================================================
    * QA
  ==========================================================================*/
  
  $('#qa').each(function() {
    $('.qaMenuInco').click(function() {
            $('.qaMenuList').toggle();
     }); 
	 
	 accordionOpen();
	//上下開合效果------------------------------------------
	function accordionOpen(){
	  $('.qa-box').each(function() {
	    $('.qa-box .qa-title a').append('<span></span>').click(function(){
			$(this).parent().toggleClass('actived').next().slideToggle();
			$('.qa-title').not($(this).parent()).removeClass('actived');
			$('.qa-info').not($(this).parent().next()).slideUp();
	    });
	  });
	}
 });

  /* ==========================================================================
    * news
  ==========================================================================*/

  $('.share-cont').each(function () {
    var $el = $(this);
    $(this).on('click', '.share', function (e) {
      e.preventDefault();
      $el.toggleClass('active');
    })
  });

  /* ==========================================================================
     * investor *Support*
   ==========================================================================*/

  function selectType() {
    $('.investor .aside').each(function () {
      var el = $(this);
      el.on('click', '.title', function () {
        el.find('.items').slideToggle(500, 'easeInOutCubic');
      });
    })
  };

  function listType() {
    $('.investor .aside').each(function () {
      var el = $(this);
      el.off('click', '.title');
      el.find('.items').css('display', '');
    })
  };

  var asideKey = 0;

  $(window).on('resize', function () {
    var mdSize = $('.max-md-size').exist();
    if (mdSize && asideKey == 0) {
      selectType();
      asideKey = asideKey + 1;
      return asideKey;
    } else if (!mdSize && asideKey == 1) {
      listType();
      asideKey = 0;
      return asideKey;
    }
  }).resize();


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
     * 指定滾動區
   ==========================================================================*/

   /*about*/
  $('#about').each(function() {

    $('.nav-sort li.a1').click(function(){
      $('html,body').animate({scrollTop:$('#abouts').offset().top - 50}, 800);
    });

    $('.nav-sort li.a2').click(function(){
      $('html,body').animate({scrollTop:$('#history').offset().top - 50}, 800);
    });

    $('.nav-sort li.a3').click(function(){
      $('html,body').animate({scrollTop:$('#core').offset().top - 50}, 800);
    });

    $('.nav-sort li.a4').click(function(){
      $('html,body').animate({scrollTop:$('#philosophy').offset().top - 50}, 800);
    });

    $('.nav-sort li.a5').click(function(){
      $('html,body').animate({scrollTop:$('#plan').offset().top - 50}, 800);
    });

  });

  /*recruit*/
  $('#recruit').each(function() {

    $('.nav-sort li.r1').click(function(){
      $('html,body').animate({scrollTop:$('#milieu').offset().top - 50}, 800);
    });

    $('.nav-sort li.r2').click(function(){
      $('html,body').animate({scrollTop:$('#welfare').offset().top - 50}, 800);
    });

  });

  /*news*/
  $('#news').each(function() {

    $('.nav-sort').click(function(){
      $('html,body').animate({scrollTop:$('#news-list').offset().top - 280}, 800);
    });

  });

  /*product*/
  $('#product').each(function() {

    $('.nav-sort').click(function(){
      $('html,body').animate({scrollTop:$('.pro_list ').offset().top - 50}, 800);
    });

  });

  /* ==========================================================================
     * header 回到最上面
   ==========================================================================*/

	  $(window).scroll(function() {

	    if ( $(this).scrollTop() > 50){
	      $("#header").addClass("actived");
	    } else {
	      $("#header").removeClass("actived");
	    }

	    //.quickly-gates-01.actived
	    if ( $(this).scrollTop() > 55){
	      $(".quickly-gates-01").addClass("actived");
	    } else {
	      $(".quickly-gates-01").removeClass("actived");
	    }

	    /*if ( $(this).scrollTop() > 100){
	      $("#header").addClass("actived-scroll");
	    } else {
	      $("#header").removeClass("actived-scroll");
	    }*/

      /*扣掉header高*/
      /*if ( $(this).scrollTop() > $("#header").height()){
        $("#header").addClass("actived-scroll");
      } else {
        $("#header").removeClass("actived-scroll");
      }*/

	  }).scroll();

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
