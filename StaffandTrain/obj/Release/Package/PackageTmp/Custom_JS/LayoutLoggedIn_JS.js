 
        $(document).ready(function () {
            if (location.pathname.split("/")[1] != "") {

                $('.navi a[href^="' + location.pathname + '"]').parent().addClass('active');
                $('.navupper a[href^="' + location.pathname + '"]').parent().addClass('active');


            }
            else {
                $('.navi a[href^="/UserHome/Index"]').parent().addClass('active');

            } 
        });
