var FormHelper = {
    clearForm: function (textFields, selectFields, radioFields, checkBoxFields) {
        for (let i = 0; i < textFields.length; i++) {
            if ($("#" + textFields[i])) {
                $("#" + textFields[i]).val("");
            }
        };

        for (let i = 0; i < selectFields.length; i++) {
            if ($("#" + selectFields[i])) {
                $("#" + selectFields[i]).val(0).trigger("change");
            }
        };

        for (let i = 0; i < radioFields.length; i++) {
            if ($(`input[name=${radioFields[i]}]`)) {
                $(`input[name=${radioFields[i]}]`).prop('checked', false);
            }
        };

        for (let i = 0; i < checkBoxFields.length; i++) {
            if ($("#" + checkBoxFields[i])) {
                $("#" + checkBoxFields[i]).prop('checked', false);
            }
        };
    },
    clearError: function (textFields, selectFields, radioFields, checkBoxFields) {
        for (let i = 0; i < textFields.length; i++) {
            if ($(`#${textFields[i]}Error`)) {
                $(`#${textFields[i]}Error`).empty();
            }
        };

        for (let i = 0; i < selectFields.length; i++) {
            if ($(`#${selectFields[i]}Error`)) {
                $(`#${selectFields[i]}Error`).empty();
            }
        };

        for (let i = 0; i < radioFields.length; i++) {
            if ($(`#${radioFields[i]}Error`)) {
                $(`#${radioFields[i]}Error`).empty();
            }
        };

        for (let i = 0; i < checkBoxFields.length; i++) {
            if ($(`#${checkBoxFields[i]}Error`)) {
                $(`#${checkBoxFields[i]}Error`).empty();
            }
        };
    },
    setValueForUpdate: function (data, textFields, selectFields, radioFields, checkBoxFields) {
        if (data == undefined || data == null) {
            FormHelper.clearForm(textFields, selectFields, radioFields, checkBoxFields);
            return false;
        }

        for (let i = 0; i < textFields.length; i++) {
            if ($("#" + textFields[i])) {
                $("#" + textFields[i]).val(data[textFields[i]] ? data[textFields[i]] : "");
            }
        }

        for (let i = 0; i < selectFields.length; i++) {
            if ($("#" + selectFields[i])) {
                $("#" + selectFields[i]).val(data[selectFields[i]] ? data[selectFields[i]] : 0).trigger("change");
            }

        };

        for (let i = 0; i < radioFields.length; i++) {
            if ($(`input[name=${radioFields[i]}]`)) {
                if (data[radioFields[i]]) {
                    $(`input[name=${radioFields[i]}][value="${data[radioFields[i]]}"]`).prop('checked', true);
                }
                else {
                    $(`input[name=${radioFields[i]}]`).prop('checked', false);
                }
            }
        };

        for (let i = 0; i < checkBoxFields.length; i++) {
            if ($("#" + checkBoxFields[i])) {
                if (data[checkBoxFields[i]]) {
                    $("#" + checkBoxFields[i]).prop('checked', true);
                    $("#" + checkBoxFields[i]).trigger("change");
                }
                else {
                    $("#" + checkBoxFields[i]).prop('checked', false);
                    $("#" + checkBoxFields[i]).trigger("change");
                }
            }
        };
    },
    getFormData: function (textFields, selectFields, radioFields, checkBoxFields) {
        var fd = new FormData();

        for (let i = 0; i < textFields.length; i++) {
            fd.append(textFields[i], $("#" + textFields[i]).val());
        };
        for (let i = 0; i < selectFields.length; i++) {
            fd.append(selectFields[i], $("#" + selectFields[i]).val());
        };
        for (let i = 0; i < radioFields.length; i++) {
            fd.append(radioFields[i], $(`input:radio[name=${radioFields[i]}]:checked`).val());
        };
        for (let i = 0; i < checkBoxFields.length; i++) {
            fd.append(checkBoxFields[i], $("#" + checkBoxFields[i]).is(":checked"));
        };

        return fd;
    },
    openModalThemMoi: function (textFields, selectFields, radioFields, checkBoxFields) {
        FormHelper.clearError(textFields, selectFields, radioFields, checkBoxFields);
        FormHelper.clearForm(textFields, selectFields, radioFields, checkBoxFields);

        setTimeout(function () {
            $("#themSuaModal .modal-body").scrollTop(0);
        }, 300);

        $("#btn-add").show();
        $("#btn-update").hide();

        $(".modal-footer button").attr("disabled", false);
        $(".modal-footer .btn-loading").addClass("invisible");
        $("#titleThemSuaModal").text("Thêm mới");
        $("#themSuaModal").modal("show");
    },
    openModalCapNhat: function (data, textFields, selectFields, radioFields, checkBoxFields) {
        FormHelper.clearError(textFields, selectFields, radioFields, checkBoxFields);
        FormHelper.clearForm(textFields, selectFields, radioFields, checkBoxFields);
        FormHelper.setValueForUpdate(data, textFields, selectFields, radioFields, checkBoxFields);

        setTimeout(function () {
            $("#themSuaModal .modal-body").scrollTop(0);
        }, 300);

        $("#btn-add").hide();
        $("#btn-update").show();

        $(".modal-footer button").attr("disabled", false);
        $(".modal-footer .btn-loading").addClass("invisible");
        $("#titleThemSuaModal").text("Cập nhật");
        $("#themSuaModal").modal("show");
    },
    openModalXoa: function (id) {
        $("#Id").val(id);
        $(".modal-footer button").attr("disabled", false);
        $(".modal-footer .btn-loading").addClass("invisible");
        $("#xoaModal").modal("show");
    },
    openModalKhoiPhuc: function (id) {
        $("#Id").val(id);
        $(".modal-footer button").attr("disabled", false);
        $(".modal-footer .btn-loading").addClass("invisible");
        $("#khoiPhucModal").modal("show");
    },
    ajaxThemMoi: function (url, fd, textFields, selectFields, radioFields, checkBoxFields) {
        FormHelper.clearError(textFields, selectFields, radioFields, checkBoxFields);

        $("#themSuaModal .modal-footer button").attr("disabled", true);
        $("#btn-loading").removeClass("invisible");

        $.ajax({
            url: url,
            data: fd,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                $("#themSuaModal .modal-footer button").attr("disabled", false);
                $("#btn-loading").addClass("invisible");

                if (data && data.Id > 0) {
                    $('#bs-table').bootstrapTable('refresh');

                    FormHelper.clearForm(textFields, selectFields, radioFields, checkBoxFields);

                    toastr.success("Xử lý thành công");
                    $("#themSuaModal").modal("hide");
                }
                else if (data && data.Errors.length > 0) {
                    data.Errors.map((item) => {
                        var idError = "#" + item.Name;
                        $(idError).show();
                        $(idError).text(item.Description);

                        return "";
                    });
                }
            },
            error: function () {
                $("#themSuaModal .modal-footer button").attr("disabled", false);
                $("#btn-loading").addClass("invisible");
                toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
            }
        });
    },
    ajaxCapNhat: function (url, fd, textFields, selectFields, radioFields, checkBoxFields) {
        FormHelper.clearError(textFields, selectFields, radioFields, checkBoxFields);

        $("#themSuaModal .modal-footer button").attr("disabled", true);
        $("#btn-loading").removeClass("invisible");

        $.ajax({
            url: url,
            data: fd,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                $("#themSuaModal .modal-footer button").attr("disabled", false);
                $("#btn-loading").addClass("invisible");

                if (data && data.Id > 0) {
                    $('#bs-table').bootstrapTable('refresh');
                    $("#themSuaModal").modal("hide");
                    toastr.success("Xử lý thành công");
                }
                else if (data && data.Errors.length > 0) {
                    data.Errors.map((item) => {
                        var idError = "#" + item.Name;
                        $(idError).show();
                        $(idError).text(item.Description);

                        return "";
                    });
                }
            },
            error: function () {
                $("#themSuaModal .modal-footer button").attr("disabled", false);
                $("#btn-loading").addClass("invisible");
                toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
            }
        });
    },
    ajaxXoaItem: function (url, id) {
        $("#xoaModal .modal-footer button").attr("disabled", true);
        $("#btn-loading").removeClass("invisible");

        var fd = new FormData();
        fd.append("Id", id);

        $.ajax({
            url: url,
            data: fd,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                $("#xoaModal .modal-footer button").attr("disabled", false);
                $("#btn-loading").addClass("invisible");

                if (data) {
                    $('#bs-table').bootstrapTable('refresh');
                    $("#xoaModal").modal("hide");
                    toastr.success("Xử lý thành công");
                } else {
                    toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
                }
            },
            error: function () {
                $("#xoaModal .modal-footer button").attr("disabled", false);
                $("#btn-loading").addClass("invisible");
                toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
            }
        });
    },
    ajaxKhoiPhucItem: function (url, id) {
        $("#khoiPhucModal .modal-footer button").attr("disabled", true);
        $("#btn-loading").removeClass("invisible");

        var fd = new FormData();
        fd.append("Id", id);

        $.ajax({
            url: url,
            data: fd,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                $("#khoiPhucModal .modal-footer button").attr("disabled", false);
                $("#btn-loading").addClass("invisible");

                if (data) {
                    $('#bs-table').bootstrapTable('refresh');
                    $("#khoiPhucModal").modal("hide");
                    toastr.success("Xử lý thành công");
                } else {
                    toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
                }
            },
            error: function () {
                $("#khoiPhucModal .modal-footer button").attr("disabled", false);
                $("#btn-loading").addClass("invisible");
                toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
            }
        });
    },
    ajaxPostData: function (url, fd, textFields, selectFields, radioFields, checkBoxFields, modalId) {
        FormHelper.clearError(textFields, selectFields, radioFields, checkBoxFields);

        $("#" + modalId + " .modal-footer button").attr("disabled", true);
        $("#btn-loading").removeClass("invisible");
        $(".btn-loading").removeClass("invisible");

        $.ajax({
            url: url,
            data: fd,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (data) {
                $("#" + modalId + " .modal-footer button").attr("disabled", false);
                $("#btn-loading").addClass("invisible");
                $(".btn-loading").addClass("invisible");

                if (data && data.Id > 0) {
                    $('#bs-table').bootstrapTable('refresh');

                    FormHelper.clearForm(textFields, selectFields, radioFields, checkBoxFields);

                    toastr.success("Xử lý thành công");
                    $("#" + modalId).modal("hide");
                }
                else if (data && data.Errors.length > 0) {
                    data.Errors.map((item) => {
                        var idError = "#" + item.Name;
                        $(idError).show();
                        $(idError).text(item.Description);

                        return "";
                    });
                }
            },
            error: function () {
                $("#" + modalId + " .modal-footer button").attr("disabled", false);
                $("#btn-loading").addClass("invisible");
                $(".btn-loading").addClass("invisible");
                toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
            }
        });
    },
    openModalCustom: function (id, modalId) {
        $("#Id").val(id);
        $(".modal-footer button").attr("disabled", false);
        $(".modal-footer .btn-loading").addClass("invisible");
        $("#" + modalId).modal("show");
    },
};

function ajaxGet(url, data, funcCallback, funcError) {
    if (typeof data === 'function') {
        funcError = funcCallback;
        funcCallback = data;
        data = null;
    }
    $.ajax({
        url: url,
        method: 'GET',
        data: data,
        success: function (res) {
            if (funcCallback) funcCallback(res);
        },
        error: function (xhr) {
            console.log(xhr);
            if (funcError) funcError(xhr);
            if (xhr.status === 401 && !window.location.pathname.includes('/dangnhap')) {
                window.location.href = '/dangnhap';
            }
        }
    });
}

function ajaxPostFormData(url, formData, funcCallback, funcError) {
    $.ajax({
        url: url,
        method: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (funcCallback) funcCallback(res);
        },
        error: function (xhr) {
            console.log(xhr)
            if (funcError) funcError(xhr);

            if (xhr.status === 401 && !window.location.pathname.includes('/dangnhap')) {
                sessionStorage.clear();
                window.location.href = '/dangnhap';
            }
        }
    });
}

function downloadFromLink(urlFile, tenFile) {
    // Lấy extension từ tên file hoặc đường dẫn
    let fileName = tenFile || urlFile;
    let extension = fileName.split('.').pop().toLowerCase();
    
    // Danh sách extension của hình ảnh và PDF
    let imageExtensions = ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg', 'ico'];
    let isImage = imageExtensions.includes(extension);
    let isPdf = extension === 'pdf';
    
    // Nếu là PDF hoặc hình ảnh thì mở trong tab mới
    if (isPdf || isImage) {
        window.open(urlFile, '_blank');
    } else {
        // Các file khác thì download
        let link = document.createElement('a');
        link.href = urlFile;
        link.download = tenFile;
        document.body.appendChild(link);
        link.click();
        link.remove();
    }
};
