


$(document).ready(function () {
    $('#txtSearch').keypress(function (e) {
        if (e.which == 13) {
            loadSearchResults($('#txtSearch').val());
            return false;    //<---- Add this line
        }
    });
    $('#txtSearch').keyup(function (e) {
        if ($('#txtSearch').val().length > 0) {
            $(".search-icon i.search").addClass("hidden");
            $(".search-icon i.clear").removeClass("hidden");
        }
        else {
            $(".search-icon i.search").removeClass("hidden");
            $(".search-icon i.clear").addClass("hidden");
        }
    });

    $(document).on("click", ".search-icon i.clear", function (e) {
        $('#txtSearch').val("");
        $("#tblCompanyResults,#tblContactResults").addClass('hidden');
    });

    $("#btnSearch").click(function (e) {
        loadSearchResults($('#txtSearch').val());
    });
    $(".company-header").click(function () {
        if ($(".company-header").hasClass("expanded")) {
            $("#tblCompanyResults thead tr:last-child").hide();
            $("#tblCompanyResults tbody").hide();
            $(".company-ec-icon").html('<i class="fa fa-angle-down"></i>');
            $(".company-header").removeClass("expanded")
        }
        else {
            $("#tblCompanyResults thead tr").show();
            $("#tblCompanyResults tbody").show();
            $(".company-ec-icon").html('<i class="fa fa-angle-up"></i>');
            $(".company-header").addClass("expanded")
        }
    });

    $(".contact-header").click(function () {
        if ($(".contact-header").hasClass("expanded")) {
            $("#tblContactResults thead tr:last-child").hide();
            $("#tblContactResults tbody").hide();
            $(".contact-ec-icon").html('<i class="fa fa-angle-down"></i>');
            $(".contact-header").removeClass("expanded")
        }
        else {
            $("#tblContactResults thead tr").show();
            $("#tblContactResults tbody").show();
            $(".contact-ec-icon").html('<i class="fa fa-angle-up"></i>');
            $(".contact-header").addClass("expanded")
        }
    });
});


function loadSearchResults(searchQuery) {

    showLoader();

    $.ajax({
        type: "POST",
        url: '/GlobalSearch/GetSearchResults',
        data: { searchQuery: searchQuery },
        beforeSend: function () { showLoader(); },
        success: function (response) {
            hideLoader();
            console.log('Ajax response ', response);
            if (response.success) {
                renderSearchResults(response);
                $("#tblCompanyResults,#tblContactResults").removeClass('hidden');
            }
        },
        error: function (error) {
            hideLoader();
        },
    });
}

function renderSearchResults(response) {
    let companies = response.companies;
    let contacts = response.contacts;
    $("#tblCompanyResults tbody, #tblContactResults tbody").html('');
    if (companies && companies.length > 0) {
        $(".company-count").html(companies.length);
        for (var i = 0; i < companies.length; i++) {
            let element = companies[i];
            $("#tblCompanyResults tbody").append(prepareCompanyRow(element));
        }
    }
    else {
        $(".company-count").html(0);
        $("#tblCompanyResults tbody").append(getBlankRow(3));
    }

    if (contacts && contacts.length > 0) {
        $(".contact-count").html(contacts.length);
        for (var i = 0; i < contacts.length; i++) {
            let element = contacts[i];
            $("#tblContactResults tbody").append(prepareContactRow(element));
        }
    }
    else {
        $(".contact-count").html(0);
        $("#tblContactResults tbody").append(getBlankRow(3));
    }
}



function prepareCompanyRow(element) {
    let actions = `<a href="/ProspectViewList/SaveCompany?companyidedit=${element.companyid}" target='_blank' class="details AnyAction"> 
                                <i class="fa fa-pencil-square-o"></i>
                               </a>`;
    let row = `<tr>
                            <td>${element.biztype ? element.biztype : ''}</td>
                            <td> <a href="/ProspectViewCompany/Index?Compid=${element.companyid}" target="_blank">
                                    ${element.name}
                                </a>
                            </td>
                            <td>
                                <a href="/ProspectViewList/Index?listid=${element.listid}&lstname=${element.listname}" target="_blank">
                                    ${element.listname}
                                </a>
                            </td>
                            <td>${actions}</td>
                           </tr>`;

    return row;
}

function prepareContactRow(element) {
    let actions = `<a href="/ProspectViewCompany/Index?Compid=${element.companyid}" target='_blank' class="details AnyAction"> 
                                View Company Details
                               </a>`;
    let row = `<tr>
                  <td>
                        <p>ID: <b>${element.contactid}</b></p>
                        <p>Name: <b>${getValue(element.contactfullname)}</b></p>
                        <p>Title: <b>${getValue(element.titlestandard)}</b></p>
                        <p>WPhone: <b>${getValue(element.contactphone)}</b></p>
                        <p>CPhone: <b>${getValue(element.contactcellphone)}</b></p>
                        <p>Email: <a href="mailto:${element.contactemail}">${getValue(element.contactemail)}</a></p>
                        <p>LinkedIn: <a target="_blank" href='${element.linkedinprofileurl}'>${element.linkedinprofileurl ? getValue(element.contactfullname) : ''}</a></p>
                  </td>
                  <td>
                      ${element.combinednotes}
                  </td>

                    <td>
                        <a href="/ProspectViewList/Index?listid=${element.listid}&lstname=${element.listname}" target="_blank">
                            ${element.listname}
                        </a>
                    </td>
                  <td>${actions}</td>
               </tr>`;

    return row;
}

function getValue(value) {
    if (value && value.length > 0) return value;
    return "";
}

function getBlankRow(colspan) {
    let blankRow = `<tr>
                     <td colspan="${colspan}" align="center"> No record(s) found</td>
                    </tr>`;
    return blankRow;
}

function showLoader() {
    $(".loading").removeClass("hidden");
}

function hideLoader() {
    $(".loading").addClass("hidden");
}