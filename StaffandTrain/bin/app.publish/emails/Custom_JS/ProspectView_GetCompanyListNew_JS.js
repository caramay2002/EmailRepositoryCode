
    $(document).ready(function () {


        var Companyid = $("#hiddencmpnyid").html();
        if (Companyid != "" && Companyid != null) {
           
            $("#" + Companyid).focus();
        } 
        //$(window).unload(function () {
        //    var scrollPosition = $("div#element").scrollTop();
        //    localStorage.setItem("scrollPosition", scrollPosition);
        //});


            if (localStorage.scrollPosition) {
                
                //alert("if=" + localStorage.getItem("scrollPosition"))
                $(window).scrollTop(localStorage.getItem("scrollPosition"))
                localStorage.setItem("scrollPosition", null);

            }
            else {
                //alert("else=" + localStorage.getItem("scrollPosition"));
            }

        
       
       

        $('.AnyAction').click(function () {
            
            var tempScrollTop = $(window).scrollTop();
            localStorage.setItem("scrollPosition", tempScrollTop);
            var s = localStorage.getItem("scrollPosition")

        });

        $('#WebGrid > table > tbody > tr').each(function (index) {
      
            if ($(this).children('td:nth-child(4)').text() == "True") {

                $(this).attr('class', 'Str_blue')
               
            }
            if ($(this).children('td:nth-child(5)').text() == "True") {

                $(this).attr('class', 'Str_pink')
            }
            $(this).children('td:nth-child(4)').hide();
            $(this).children('td:nth-child(5)').hide();

        })

        $('#WebGrid > table > thead > tr > th').each(function (index) {
            if ($(this).text().trim() == "priority") {
                $(this).hide();
            }
            if ($(this).text().trim() == "Target") {
                $(this).hide();
            }
        })

       
});

 



