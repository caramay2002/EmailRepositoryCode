


$(document).ready(function () {

            if (location.pathname.split("/")[1] != "") {

                $('.navi a[href^="' + location.pathname + '"]').parent().addClass('active');
                $('.navupper a[href^="' + location.pathname + '"]').parent().addClass('active');


            }
            else {
                $('.navi a[href^="/UserHome/Index"]').parent().addClass('active');

            }

        });
 //function load_css_async(filename) {


 //       var cb = function () { 
 //           var l = document.createElement('link'); l.rel = 'stylesheet';
 //           l.href = filename;
 //           var h = document.getElementsByTagName('head')[0]; h.parentNode.insertBefore(l, h);
 //       };
 //       var raf = requestAnimationFrame || mozRequestAnimationFrame ||
 //           webkitRequestAnimationFrame || msRequestAnimationFrame;
 //       if (raf) raf(cb);
 //       else window.addEventListener('load', cb);


 //   }