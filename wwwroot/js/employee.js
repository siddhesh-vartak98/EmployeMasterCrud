$(document).ready(function () {


    var phoneNo = document.querySelector("#MobileNo");

    var iti; var itiTwo;

    iti = window.intlTelInput(phoneNo, {
        initialCountry: "auto",
        formatOnDisplay: true,
        separateDialCode: true,
        autoHideDialCode: true,
        geoIpLookup: callback => {
            fetch("https://ipapi.co/json")
                .then(res => res.json())
                .then(data => callback(data.country_code))
                .catch(() => callback("us"));
        },
        utilsScript: "https://cdn.jsdelivr.net/npm/intl-tel-input@18.1.1/build/js/intlTelInput.min.js" // just for formatting/placeholders etc
    });

    var countryData = iti.getSelectedCountryData();

    console.log(countryData);

    console.log(countryData.dialCode);

    if (countryData != null) {

        CountryCode = countryData.dialCode;
        var countryFlag = countryData.iso2;

        if (CountryCode !== undefined) {
            $("#CountryCode").val(CountryCode);
            $("#CountryFlag").val(countryFlag);
        } else {
            
        }
    } else {
        console.log("country not get;")
    }

    //Multiple Image Uplaod

    $(".btnMultiUploadImage").unbind().click(function () {

        /* alert("in js");*/

        var itemCount = 0;
        var Path = $(this).attr("data-path");
        var AjaxURL = "/EmployeeMaster/MultiUploadFile";
        var file = $(".fumulti").prop("files");
        var fileUpload = $("#fumultiimage");

        var formData = new FormData();

        $.each(file, function (i, j) {

            console.log("i: " + i);
            console.log("j: " + j);

            formData.append(j.name, j);

        });

        if (file != null) {

            if (IsValidImages(fileUpload)) {

                console.log(formData);

                var _URL = window.URL || window.webkitURL;
                var file, img, width, height, size;
                size = file.size;

                //var formData = new FormData();
                //formData.append(file.name, file);

                $.ajax({
                    type: "POST",
                    url: AjaxURL,
                    data: formData,
                    processData: false,
                    contentType: false,
                    beforeSend: function () {
                        $("#btnMultiUploadImage").attr({ 'value': 'Please wait..' });
                    },
                    success: function (data) {

                        const images = data.split(',');

                        //alert(images);

                        $("#btnMultiUploadImage").attr({ 'value': 'Upload' });
                        if (data == "") {
                            alert("Oops some error occured.");
                        }
                        else {

                            images.forEach(element => {

                                //console.log(element);

                                itemCount = $("#hfGalleryFileCount").val();

                                itemCount++;

                                $("#hfGalleryFileCount").val(itemCount);

                                //appendItem(element, itemCount, Path);
                                appendItemMlImg(element, itemCount, Path);

                                fileUpload.val('');
                            });
                        }
                    }
                });

            }
            else {
                alert("Please upload a valid file.");
            }

        }
        else {
            alert("Please upload a file.");
        }
    });

    $("#addFileNewsDownload").unbind().click(function (e) {

        e.preventDefault();
        var count = $("#hfNewsFileCount").val();

        count++;

        appendTaskFormNewsDownload(count);

        var ddlDocType = $('#partyDocumentTypeID_@count');

        //$.ajax({
        //    type: "POST",
        //    url: "/Ajax/GetDocumentTypes",
        //    dataType: "json",
        //    success: function (data) {

        //        //console.log(data);
        //        if (data != "") {
        //            $.each(data, function (key, value) {
        //                $(ddlDocType).append($("<option></option>").val(value.partyDocumentTypeID).html(value.partyDocumentTypeName));
        //            });
        //        }
        //        else {
        //            $(ddlDocType).empty().append('<option selected="selected" value="">Select </option>');
        //        }
        //    }
        //});

        $("#hfNewsFileCount").val(count);
    });

    $(document).on("click", ".btnNewsDownloadFile", function () {
        //alert();

        var AjaxURL = $(this).attr("data-url");
        var imagePath = $(this).attr("data-path");
        var fileSizeAllow = 1024;

        var Count = $(this).attr("data-count");

        var file = $('#fuNewsFile_@count').prop("files")[0];
        var fileUpload = $('#fuNewsFile_@count');

        /*var count = $("#hfExDownloadFileCount").val();*/

        var selectedOptionText = $('#txtDocumentName_' + Count).val();

        var fileTitle = $("#txtDocumentNumber_" + Count).val();

        if (selectedOptionText != null && selectedOptionText != "") {
            if (fileTitle != null && fileTitle != "") {
                if (file != null) {

                    var fileName = file.name;

                    var fileArrry = fileName.split('.');

                    //console.log(fileArrry);

                    var lengthOfFile = fileArrry.length;

                    //console.log(lengthOfFile);

                    if (lengthOfFile > 2) {
                        alert("Please remove Dot in file Name.")
                    }
                    else if (lengthOfFile == 2) {
                        //console.log(file);
                        //console.log(fileName);
                        //console.log(fileUpload);

                        if (IsValidFile(fileUpload)) {

                            var img, width, height, size;
                            size = file.size / 1024;

                            if (size < fileSizeAllow) {
                                var formData = new FormData();
                                formData.append(file.name, file);
                                //alert(file);
                                $.ajax({
                                    type: "POST",
                                    url: AjaxURL,
                                    data: formData,
                                    processData: false,
                                    contentType: false,
                                    success: function (data) {
                                        //alert(data);
                                        if (data == "") {
                                            alert("Oops some error occured.");
                                        }
                                        else {
                                            //console.log(data);

                                            var title = $("#txtDocumentNumber_" + Count).val();

                                            appendVariantNewsDownload(data, Count, title, imagePath)

                                            var fileUpload = $('#fuNewsFile_@count');

                                            fileUpload.val('');

                                            alert("File Attached Successfully.");
                                        }
                                    }
                                });

                            }
                            else {
                                alert("Please upload a valid image with less than 1 MB.");
                            }

                        } else {
                            alert("Please upload a valid File.");
                        }

                    }
                    else {
                        alert("Please select a file.");
                    }
                }
                else {
                    alert("Please select a file.");
                }
            }
            else {
                alert("Please input document number.");
            }
        }
        else {
            alert("Please select document name");
        }

    });

    $(document).on("click", ".removeNewsFile", function () {

        //$(this).parent().parent().parent().remove();
        $(this).parent().parent().remove();
    });

    $(".deleteModelBox").click(function () {

        $("#id").val($(this).attr("data-id"));

        $("#modal-deleteBox").modal();

    });

    $(document).on("click", ".divDeleteFileNewsDownlaod", function () {
        /* $(this).parent().remove();*/
        var Count = $(this).attr("data-count");

        $(this).parent().parent().remove();

        //$("txtFilePackage_" + Count +"").removeClass();
    });


});

function IsValidFile(file) {
    var FileExtension = $(file).val().split('.')[1];
    if (FileExtension == "jpg" || FileExtension == "jpeg" || FileExtension == "png" || FileExtension == "JPG" || FileExtension == "JPEG" || FileExtension == "PNG" || FileExtension == "DOC" || FileExtension == "doc" || FileExtension == "DOCX" || FileExtension == "docx" || FileExtension == "PDF" || FileExtension == "pdf") { return true; }
    else { return false; }
}

function IsValidImages(file) {
    var FileExtension = $(file).val().split('.')[1];
    if (FileExtension == "jpg" || FileExtension == "jpeg" || FileExtension == "png" || FileExtension == "JPG" || FileExtension == "JPEG" || FileExtension == "PNG") { return true; }
    else { return false; }
}

function appendItemMlImg(imageName, count, path) {

    var imageHTML = '<div class="divImage">' +
        '<input type="hidden" name="hf_image_@count" value="' + imageName + '" />' +
        '<img src = "' + path + imageName + '" alt = "AlternateText" id = "MultiUploadImage" height = "150"/>' +
        '<div class="divDeleteImage">' +
        '<span class="fa fa-trash"></span>' +
        '</div>' +
        '</div >';

    $(".divUploadedItemContainer").append(imageHTML);
}

function appendTaskFormNewsDownload(count) {

    var imageHTML = '<div class="row mt-2" name="txtPackage_@count">' +
        '<div class="col-md-2">' +
        '<input type="text" class="form-control" id="txtDocumentName_@count" name="txtDocumentName_@count" placeholder="Enter Document Name" maxlength="30" aria-describedby="documentTitleHelpBlock" />' +
        '<small id="documentTitleHelpBlock" class="form-text text-muted">' +
        'Document name has 30 characters limit.' +
        '</small>' +
        '<div class="divError">' +

        '</div>' +
        '</div>' +

        '<div class="col-md-3">' +

        '<input type="text" class="form-control" name="txtDocumentNumber_@count" id="txtDocumentNumber_@count" placeholder="Enter Document Number" maxlength="20" aria-describedby="newsDownTitleHelpBlock" />' +
        '<small id="newsDownTitleHelpBlock" class="form-text text-muted">' +
        'Document number has 20 characters limit.' +
        '</small>' +
        '<div class="divError">' +

        '</div>' +
        '</div>' +

        '<div class="col-md-4">' +

        '<table>' +
        '<tr>' +
        '<td>' +
        '<input type="file" id="fuNewsFile_@count" class="fuNewsFile_@count" accept="application/msword,.docx, application/pdf, .jpg,.jpeg,.png " style="width:100%;" />' +
        '</td>' +

        '<td>' +
        '<input type="button" id="btnNewsDownloadFile" value="Upload" class="btn btn-primary btnNewsDownloadFile" data-count="@count" data-URL="/EmployeeMaster/SingleUploadFile" data-path="/Content/DocumentFiles" />' +
        '</td>' +
        '</tr>' +

        '<tr style="font-size:13px;height: 13px!important;">' +
        '<td class="">' +
        ' Please upload file size less than 1MB.' +
        '</td>' +
        '</tr>' +

        '</table>' +
        '</div>' +

        '<div id="newsDownloadFileContainer_@count" class="col-md-2">' +
        '</div>' +

        '<div  class="col-md-1">' +

        '<button title="Delete" type="button" class="btn btn-primary btn-sm removeNewsFile" data-deleteCount="@count" data-toggle="tooltip" data-placement="top"> <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash-fill" viewBox="0 0 16 16"> <path d="M2.5 1a1 1 0 0 0-1 1v1a1 1 0 0 0 1 1H3v9a2 2 0 0 0 2 2h6a2 2 0 0 0 2-2V4h.5a1 1 0 0 0 1-1V2a1 1 0 0 0-1-1H10a1 1 0 0 0-1-1H7a1 1 0 0 0-1 1H2.5zm3 4a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 .5-.5zM8 5a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7A.5.5 0 0 1 8 5zm3 .5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 1 0z" /></svg> </button>' +

        '</div>' +

        '</div>';


    $("#newsDownloadFileShowContainer").append(imageHTML);
}

function appendVariantNewsDownload(data, count, title, path) {

    var imageHTML = '<div class="divImage">' +
        '<input type="hidden" name="hf_documentFile_@count" value="' + data + '" />' +
        '<a href="' + path + data + '"><img src = "/images/fileImage.svg" alt = "AlternateText" id = "MultiUploadImage" height = "100"/></a>' +
        '<div class="divDeleteImage">' +
        '<span class="fa fa-trash"></span>' +
        '</div>' +
        '</div >';

    $("#newsDownloadFileContainer_" + count).append(imageHTML);
}


