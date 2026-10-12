"use strict";
var selectedId = 0;
let phanQuyenData = [];
function rowAttributes(row, index) {
    return {
        'data-id': row.Id
    }
}

function operateFormatter(value, row, index) {
    const btnSua = `<button id='btn-update-item-2' class='btn btn-primary btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-pencil'></i> Sửa</button>`;
    const btnGanVaiTroKhac = `<button id='btn-open-assign-other-role' class='btn btn-info btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-user-plus'></i> Gán vai trò khác</button>`;
    const btnXoa = `<button id='btn-delete-item' class='btn btn-danger btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-trash'></i> Xóa</button>`;
    const btnKhoiPhuc = `<button id='btn-revert-item' class='btn btn-success btn-sm m-l-5 m-r-5 m-t-5 m-b-5'><i class='fa fa-refresh'></i> Khôi phục</button>`;

    if (row.IsDeleted) {
        return [
            btnSua + btnKhoiPhuc
        ].join("");
    }

    return [
        btnSua + btnGanVaiTroKhac + btnXoa
    ].join("");
}

//Open Model when update
window.operateEvents = {
    'click #btn-update-item-2': function (e, value, row, index) {
        clearError();
        clearForm();

        $("#btn-add").hide();
        $("#btn-update").show();
        selectedId = row.Id;
        $("#Ten").val(row.Ten);

        const phanQuyenKeys = row.PhanQuyenKeys ? row.PhanQuyenKeys.split(",") : "";
        for (let i = 0; i < phanQuyenKeys.length; i++) {
            $("#" + phanQuyenKeys[i].trim()).click();
        }

        $(".modal-footer button").attr("disabled", false);
        $("#btn-loading").addClass("invisible");
        $("#titleThemSuaModal").text("Cập nhật");
        $("#themSuaModal").modal("show");
    },
    'click #btn-delete-item': function (e, value, row, index) {
        selectedId = row.Id;
        $("#maCVDelete").text(row.Ten);
        $(".modal-footer button").attr("disabled", false);
        $("#btn-loading").addClass("invisible");
        $("#xoaModal").modal("show");
    },
    'click #btn-open-assign-other-role': function (e, value, row, index) {
        selectedId = row.Id;
        $("#tenVaiTroGan").text(row.Ten);
        $("#GanPhongBanIdsError").hide().text("");
        clearGanVaiTroKhacForm();
        openGanVaiTroKhacButtonFooter();
        $("#ganVaiTroKhacModal").modal("show");
    },
    'click #btn-revert-item': function (e, value, row, index) {
        selectedId = row.Id;
        $("#hoTenKP").text(row.HoTen);
        $(".modal-footer button").attr("disabled", false);
        $("#btn-loading").addClass("invisible");
        $("#khoiPhucModal").modal("show");
    }
}

function queryParams(params) {
    params.search = btoa(unescape(encodeURIComponent($("#txt-search").val())));
    params.DaXoaSearch = $('#DaXoaSearch').is(":checked");
    return params;
};

function search() {
    $('#bs-table').bootstrapTable('refresh');
};

function enterSearch(e) {
    if (e.keyCode === 13) {
        $('#bs-table').bootstrapTable('refresh');
    };
};

$("#DaXoaSearch").change(function () {
    search();
})

function clearError() {
    $("#TenError").hide();
}
function clearForm() {
    $('#vaiTroCheckboxList').find('input:checkbox').prop('checked', false);
    $("#Ten").val("");
}

function clearGanVaiTroKhacForm() {
    $("#GanPhongBanIds_options").find('input[type="checkbox"]').prop("checked", false).prop("indeterminate", false);
    $("#GanPhongBanIds_search").val("");

    if (typeof window["filterMultiSelectOptions_GanPhongBanIds"] === "function") {
        window["filterMultiSelectOptions_GanPhongBanIds"]();
    }

    if (typeof window["updateMultiSelectPlaceholder_GanPhongBanIds"] === "function") {
        window["updateMultiSelectPlaceholder_GanPhongBanIds"]();
    }

    if (typeof window["updateMultiSelectSelectAllState_GanPhongBanIds"] === "function") {
        window["updateMultiSelectSelectAllState_GanPhongBanIds"]();
    }
}

function openButtonFooter() {
    $(".modal-footer button").attr("disabled", false);
    $("#btn-loading").addClass("invisible");
}

function openGanVaiTroKhacButtonFooter() {
    $("#ganVaiTroKhacModal .modal-footer button").attr("disabled", false);
    $("#btn-loading-gan-vai-tro").addClass("invisible");
}

function getSelectedGanPhongBanIds() {
    return $("#GanPhongBanIds_options")
        .find('input[type="checkbox"]:not([value="all"]):checked')
        .map(function () {
            return $(this).val();
        })
        .get()
        .join(",");
}

//Open Model when create
$("#btn-open-modal").click(function () {
    clearForm();
    clearError();
    $("#btn-add").show();
    $("#btn-update").hide();
    $(".modal-footer button").attr("disabled", false);
    $("#btn-loading").addClass("invisible");
    $("#titleThemSuaModal").text("Thêm mới");
    $("#themSuaModal").modal("show");
});

function getFormData() {
    var fd = new FormData();
    fd.append("Id", selectedId);
    fd.append("Ten", $("#Ten").val());

    let vaiTroIds = $('#vaiTroCheckboxList input:checked').map(function () {
        return $(this).val();
    }).get();

    fd.append("PhanQuyenStr", JSON.stringify(vaiTroIds));//vaiTroIds.toString()
    return fd;
}

//add item
$("#btn-add").click(function () {
    clearError();
    $(".modal-footer button").attr("disabled", true);
    $("#btn-loading").removeClass("invisible");

    $.ajax({
        url: "/VaiTro/Them",
        data: getFormData(),
        processData: false,
        contentType: false,
        type: "POST",
        success: function (data) {
            if (data && data.Id > 0) {
                $('#bs-table').bootstrapTable('refresh');
                clearForm();
                $(".modal-footer button").attr("disabled", false);
                $("#btn-loading").addClass("invisible");
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
                openButtonFooter();
            }
        },
        error: function () {
            toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
        }
    });
});

//update item
$("#btn-update").click(function () {
    clearError();
    $(".modal-footer button").attr("disabled", true);
    $("#btn-loading").removeClass("invisible");

    $.ajax({
        url: "/VaiTro/CapNhat",
        data: getFormData(),
        processData: false,
        contentType: false,
        type: "POST",
        success: function (data) {
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
                openButtonFooter();
            }
        },
        error: function () {
            toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
        }
    });
});

$("#btn-assign-other-role").click(function () {
    $("#GanPhongBanIdsError").hide().text("");
    const phongBanIds = getSelectedGanPhongBanIds();

    $("#ganVaiTroKhacModal .modal-footer button").attr("disabled", true);
    $("#btn-loading-gan-vai-tro").removeClass("invisible");

    var fd = new FormData();
    fd.append("VaiTroId", selectedId);
    fd.append("PhongBanIdsStr", phongBanIds);

    ajaxPostFormData("/VaiTro/GanVaiTroKhacChoPhongBan", fd, function funcCallback(data) {
        if (data && data.Success) {
            $("#ganVaiTroKhacModal").modal("hide");
            toastr.success(data.Message || "Xử lý thành công");
        } else {
            $("#GanPhongBanIdsError").show().text((data && data.Message) || "Đã có lỗi xảy ra. Vui lòng tải lại trang.");
            openGanVaiTroKhacButtonFooter();
        }
    }, function funcError() {
        toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
        openGanVaiTroKhacButtonFooter();
    });
});

//delete item
$("#btn-delete").click(function () {
    $(".modal-footer button").attr("disabled", true);
    $("#btn-loading").removeClass("invisible");

    var fd = new FormData();
    fd.append("Id", selectedId);

    $.ajax({
        url: "/VaiTro/Xoa",
        data: fd,
        processData: false,
        contentType: false,
        type: "POST",
        success: function (data) {
            if (data) {
                $('#bs-table').bootstrapTable('refresh');
                $("#xoaModal").modal("hide");
                toastr.success("Xử lý thành công");
            } else {
                toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
            }
        },
        error: function () {
            toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
        }
    });
});

$("#btn-revert").click(function () {
    $(".modal-footer button").attr("disabled", true);
    $("#btn-loading").removeClass("invisible");

    var fd = new FormData();
    fd.append("Id", selectedId);

    $.ajax({
        url: "/VaiTro/KhoiPhuc",
        data: fd,
        processData: false,
        contentType: false,
        type: "POST",
        success: function (data) {
            if (data) {
                $('#bs-table').bootstrapTable('refresh');
                $("#khoiPhucModal").modal("hide");
                toastr.success("Xử lý thành công");
            } else {
                toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
            }
        },
        error: function () {
            toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
        }
    });
});
