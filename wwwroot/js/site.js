$(document).ready(function () {
    window.setTimeout(function () {
        $(".alert-success").fadeTo(500, 0).slideUp(500, function () {
            $(this).remove();
        });
    }, 20000);

    window.setTimeout(function () {
        $(".alert-danger").fadeTo(500, 0).slideUp(500, function () {
            $(this).remove();
        });
    }, 40000);

    window.setTimeout(function () {
        $(".alert-warning").fadeTo(500, 0).slideUp(500, function () {
            $(this).remove();
        });
    }, 3000);

    $('#CountryId').select2();
    $('#StateId').select2();
    $('#CityId').select2();

    // Remove active for all items.
    $('.page-sidebar-menu li').removeClass('active');

    // highlight submenu item
    $('li a[href="/' + this.location.pathname.split('/')[1] + '"]').parent().addClass('active');

    // Highlight parent menu item.
    $('ul a[href="/' + this.location.pathname.split('/')[1] + '"]').parents('li').addClass('active');

    //Cascading State list
    $("#CountryId").change(function () {
        /* alert("Country select");*/

        var selectedValue = $("#CountryId option:selected").val();

        var ddlStates = $('#StateId');
        console.log(selectedValue);
        if (selectedValue != '') {

            $(ddlStates).empty().append('<option selected="selected" value="">Select State</option>');

            /*here in $(ddlcity) is ineter link to  county-state*/
            $('#cityId').empty().append('<option selected="selected" value="">Select</option>');


            $.ajax({
                type: "POST",
                url: "/Ajax/GetStates",
                dataType: "json",
                data: { "CountryId": selectedValue },
                success: function (data) {

                    console.log(data);
                    if (data != "") {
                        $.each(data, function (key, value) {
                            $(ddlStates).append($("<option></option>").val(value.StateId).html(value.StateName));
                        });
                    }
                    else {
                        $(ddlStates).empty().append('<option selected="selected" value="">Select State</option>');
                    }
                }
            });
        }
        else {
            $(ddlStates).empty().append('<option selected="selected" value="">Select State</option>');
        }

    });
    // End Cascading list

    //Cascading State list
    $("#StateId").change(function () {
        /* alert("Country select");*/

        var selectedValue = $("#StateId option:selected").val();

        var ddlStates = $('#CityId');
        console.log(selectedValue);
        if (selectedValue != '') {

            $(ddlStates).empty().append('<option selected="selected" value="">Select City</option>');

            /*here in $(ddlcity) is ineter link to  county-state*/
            //$('#cityId').empty().append('<option selected="selected" value="">Select City</option>');

            $.ajax({
                type: "POST",
                url: "/Ajax/GetCities",
                dataType: "json",
                data: { "StateID": selectedValue },
                success: function (data) {

                    console.log(data);
                    if (data != "") {
                        $.each(data, function (key, value) {
                            $(ddlStates).append($("<option></option>").val(value.CityId).html(value.CityName));
                        });
                    }
                    else {
                        $(ddlStates).empty().append('<option selected="selected" value="">Select City</option>');
                    }
                }
            });
        }
        else {
            $(ddlStates).empty().append('<option selected="selected" value="">Select City</option>');
        }
    });
    // End Cascading list


    //for single image upload
    $("#btnUploadImage").unbind().click(function (e) {

        var AjaxURL = $(this).attr("data-URL");
        var file = $(".fu").prop("files")[0];
        /*alert(file);*/
        var imagePath = $(this).attr("data-path");

        //console.log(file);
        if (file != null) {

            if (IsValidImage(file)) {
                var img, width, height, size;
                size = file.size / 1024;

                //console.log(size);

                if (size < 1024) {
                    var formData = new FormData();
                    formData.append(file.name, file);

                    $.ajax({
                        type: "POST",
                        url: AjaxURL,
                        data: formData,
                        processData: false,
                        contentType: false,
                        beforeSend: function () {
                            $("#btnUploadImage").attr({ 'value': 'Please wait..' });
                        },
                        success: function (data) {
                            $("#btnUploadImage").attr({ 'value': 'Upload' });

                            /* alert()*/
                            if (data == "") {
                                alert("Oops some error occured.");
                            }
                            else {
                                /*alert(data);*/

                                $("#ImgUploadedImage").attr("src", imagePath + "/" + data);
                                $("#image").val(data);

                                alert("Image Upload successfully");

                            }
                        }
                    });
                }
                else {
                    alert("Please upload a valid image with less than 1MB.");
                }
            }
            else {
                alert("Please upload a valid image.");
            }
        }
        else {
            alert("Please select a file.");
        }
    });
    /*for single image upload end*/

    //for single file upload
    $("#btnUploadMediaFile").unbind().click(function (e) {

        var AjaxURL = $(this).attr("data-URL");
        var file = $(".fu").prop("files")[0];
        /*alert(file);*/
        var filePath = $(this).attr("data-path");
        var actionMethod = $(this).attr("data-action");

        //console.log(file);
        if (file != null) {

            var formData = new FormData();
            formData.append(file.name, file);

            var fileName = file.name;

            $.ajax({
                type: "POST",
                url: AjaxURL,
                data: formData,
                processData: false,
                contentType: false,
                beforeSend: function () {
                    $("#btnUploadMediaFile").attr({ 'value': 'Please wait..' });
                },
                success: function (data) {
                    $("#btnUploadMediaFile").attr({ 'value': 'Upload' });
                    /* alert()*/
                    if (data == "") {
                        alert("Oops some error occured.");
                    }
                    else {

                        if (actionMethod == "Edit") {
                            var element = document.getElementById('downloadFiles');

                            element.remove();

                        }

                        appendMediaFile(data, fileName, filePath)

                        $("#mediaFile").val(data);

                        //console.log("data:" + data + " fileName:" + fileName + " filePath:" + filePath +" actionMethod:" + actionMethod);

                        alert("File Upload successfully");

                    }
                }
            });
        }
        else {
            alert("Please select a file.");
        }

    });
    /*for single file upload end*/

    $(document).on("click", ".statusChange", function () {

        //alert("click");

        let checkboxID = $(this).attr("data-id");
        let isChecked = $(this).is(':checked');

        //var url = window.location.href;
        var Page = 1;

        //var pageName = url.split('/')[3];

        var controllerName = pageName.split('?')[0];
        //var urlTwo = pageName.split('?')[1];

        //alert(controllerName);

        const urlParams = new URL(url).searchParams;

        const currentPage = urlParams.get('page');
        //var searchString = urlParams.get('searchString');
        //var currentFilter = urlParams.get('currentFilter');
        //var sortOrder = urlParams.get('sortOrder');
        //const ddlStatus = urlParams.get('ddlStatus');

        if (currentPage != null) {
            Page = currentPage;
        }

        var AjaxURL = controllerName + '/ChangeStatus';

        if (checkboxID != '') {

            $.ajax({
                url: AjaxURL,
                data: { "id": checkboxID },
                type: 'Get',
                success: function (response) {

                    if (response.success == true) {
                        showAlert("<strong>Success ! </strong> " + response.message, "success", 2000);

                    }
                    else {
                        showAlert("<strong>Error! ! </strong> " + response.message, "danger", 2000);

                    }


                    //console.log(response.message);
                    // Handle your response here
                },
                error: function (xhr, status, error) {
                    //console.error(error);
                    alert(error);
                }
            });

            //window.location = '/PartyTypeMaster/ChangeStatus?id=' + checkboxID;

            //var newurl = '/' + controllerName + '/ChangeStatus?id=' + checkboxID + '&page=' + Page + '&searchString=' + searchString +
            //    '&sortOrder=' + sortOrder +
            //    '&currentFilter=' + currentFilter +
            //    '&ddlStatus=' + ddlStatus;

            ////console.log(newurl);

            //window.location = newurl;

            //alert("Status Updated");

        } else {
            alert("Status ID not selected.");
        }

    });

    $(".ancAddNewVideo").unbind().click(function (e) {

        e.preventDefault();

        var count = $("#hfVideoCount").val();

        count++;

        appendVideoForm(count);
        $("#hfVideoCount").val(count);

    });
});

function IsValidImage(file) {
    var fileName = file.name;
    var FileExtension = fileName.split('.')[1];
    if (FileExtension == "jpg" || FileExtension == "jpeg" || FileExtension == "png" || FileExtension == "JPG" || FileExtension == "JPEG" || FileExtension == "PNG") { return true; }
    else { return false; }
}

// For Active Controller Name
var url = window.location.href;

var pageName = url.split('/')[3];
pageName = pageName.split('?')[0];

if (pageName == '' || pageName == 'Home') {

    pageName = "Home";

    var ancElement = $("#accordionSidebar").find("[data-name='" + pageName + "']");
    ancElement.parent().parent().addClass("show");
    ancElement.parent().parent().parent().addClass("active");
    ancElement.addClass("active");

    var ancElement2 = $(".nav-item").find("[data-name='" + pageName + "']");
    ancElement2.parent().addClass("active");
} else {
    var ancElement = $("#accordionSidebar").find("[data-name='" + pageName + "']");
    ancElement.parent().parent().addClass("show");
    ancElement.parent().parent().parent().addClass("active");
    ancElement.addClass("active");


    var ancElement2 = $(".nav-item").find("[data-name='" + pageName + "']");
    ancElement2.parent().addClass("active");

}  //End Active

function IsValidImage(file) {
    var fileName = file.name;
    var FileExtension = fileName.split('.')[1];
    if (FileExtension == "jpg" || FileExtension == "jpeg" || FileExtension == "png" || FileExtension == "JPG" || FileExtension == "JPEG" || FileExtension == "PNG") { return true; }
    else { return false; }
}

function IsValidMediaFile(file) {
    var FileExtension = $(file).val().split('.')[1];
    if (FileExtension == "jpg" || FileExtension == "jpeg" || FileExtension == "png" || FileExtension == "JPG" || FileExtension == "JPEG" || FileExtension == "PNG" || FileExtension == ".mp4" || FileExtension == ".gif" || FileExtension == ".mp3") { return true; }
    else { return false; }
}

function IsValidFile(file) {
    var FileExtension = $(file).val().split('.')[1];
    if (FileExtension == "jpg" || FileExtension == "jpeg" || FileExtension == "png" || FileExtension == "JPG" || FileExtension == "JPEG" || FileExtension == "PNG" || FileExtension == "DOC" || FileExtension == "doc" || FileExtension == "DOCX" || FileExtension == "docx" || FileExtension == "PDF" || FileExtension == "pdf") { return true; }
    else { return false; }
}

function appendMediaFile(fileName, uploadFileName, urlFile) {
    var fileInnerHTML = '<div id="downloadFiles" class="downloadFile">' +
        '    <div class="row">                        ' +
        '        <div class="col-md-12">              ' +
        '            <div class="divFileBox">         ' +
        '                <a href="' + urlFile + '/' + fileName + '" target="_blank" title="' + uploadFileName + '">' +
        '                    <img src="/images/fileImage.svg" alt="FileImage" width="90" />' +
        '                </a>                                                              ' +
        '                <div class="divDeleteFile divDeleteFileNewsDownlaod">             ' +
        '                    <span class="fa fa-trash"></span>                             ' +
        '                </div>                                                            ' +
        '            </div>                                                                ' +
        '        </div>                                                                    ' +
        '    </div>' +
        '</div>';

    $(".downloadFileContainer").append(fileInnerHTML);

}

function showAlert(message, type, closeDelay) {

    var $cont = $("#alerts-container");

    if ($cont.length == 0) {
        // alerts-container does not exist, create it
        $cont = $('<div id="alerts-container">')
            .css({
                position: "fixed"
                , width: "auto"
                , right: "10rem"
                , top: "5rem"
            })
            .appendTo($("body"));
    }

    // default to alert-info; other options include success, warning, danger
    type = type || "info";

    // create the alert div
    var alert = $('<div>')
        .addClass("fade in show alert alert-" + type)
        .append(
            $('<button type="button" class="close" data-dismiss="alert">')
                .append('&nbsp;&nbsp;<span aria-hidden="true">×</span>')
        )
        .append(message);

    // add the alert div to top of alerts-container, use append() to add to bottom
    $cont.prepend(alert);

    // if closeDelay was passed - set a timeout to close the alert
    if (closeDelay)
        window.setTimeout(function () { alert.alert("close") }, closeDelay);
} //Bootstrap Alert For Javascript

function appendVideoForm(count) {

    var docHTML = "<div class='row mb-3'>" +
        "<div class='col-md-3'>" +
        "<input type='text' name='txtVideoTitle_" + count + "' class='form-control' placeholder='Enter Video Title' />" +
        "</div>" +
        "<div class='col-md-3'>" +
        "<input type='text' name='txtVideoID_" + count + "' class='form-control' placeholder='Enter Youtube ID' />" +
        "</div>" +
        "<div class='col-md-2'>" +
        "<a class='ancRemoveVideo btn btn-danger'>Remove</a>" +
        "</div>" +
        "</div >";

    $(".divProductVideoContainer").append(docHTML);
}