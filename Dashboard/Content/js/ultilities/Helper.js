function getSelectedNguoiDungHelper(array, newArraySelected, arraySelected) {
    setTimeout(function () {

        var dataSelectNguoiDung = array;
        if (dataSelectNguoiDung) {
            var temp = [];
            for (var e = 0; e < dataSelectNguoiDung.length; e++) {
                var target = { Id: parseInt(dataSelectNguoiDung[e]) };
                if (target) {
                    temp.push(target);
                }

            }
            newArraySelected = newArraySelected.concat(temp);
        }
        if (arraySelected) {
            newArraySelected = newArraySelected.concat(arraySelected);
        }

        newArraySelected = newArraySelected.reduce((acc, current) => {
            const x = acc.find(item => item.Id === current.Id);
            if (!x) {
                return acc.concat([current]);
            } else {
                return acc;
            }
        }, []);

        for (var i = 0; i < newArraySelected.length; i++) {
            var idCheckbox = '[data-id="' + newArraySelected[i].Id + '"] .bs-checkbox';
            $(idCheckbox).click(function () {

            }).trigger('click')
        }

    }, 500);
}

function getValueDateDDMMYYYY(val) {
    if (val) {
        return moment(val).format("DD/MM/YYYY");
    }

    return null;
}

function getValueDateYYYYMMDD(val) {
    if (val) {
        return moment(val, 'DD-MM-YYYY').format('YYYY-MM-DD');
    }

    return null;
}

function setTrangThai(id, trangThai, idTable = "bs-table") {
    if (trangThai == "DaXem") return;
    $.ajax({
        url: "/NhanVanBan/SetTrangThai",
        data: { id: id },
        type: "POST",
        success: function (data) {
            if (data) {
                $('#' + idTable).bootstrapTable('refresh');
            } else {
                toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
            }
        },
        error: function () {
            toastr.error("Đã có lỗi xảy ra. Vui lòng tải lại trang.");
        }
    });
}

function trangThaiDatXeFormatter(value, row, index) {
    let lichXeTuLichHop = row.CuocHopId ? `<span class="badge badge-info">Lịch xe từ lịch họp</span>` : ``;

    if (row.TrangThaiXuLy == "ChoDuyetNoiTinh" || row.TrangThaiXuLy == "ChoDuyetNgoaiTinh") {
        return `<h5>${lichXeTuLichHop}<span class="badge badge-info">Chờ duyệt</span></h5>`;
    }
    else if (row.TrangThaiXuLy == "TraDuyet") {
        return `<h5><span class="badge badge-warning">Trả phiếu</span></h5>`;
    }
    else if (row.TrangThaiXuLy == "DaDuyet") {
        return `<h5>${lichXeTuLichHop}<span class="badge badge-info">Chờ cấp xe</span></h5>`;
    }
    else if (row.TrangThaiXuLy == "DaCapXe") {
        return `<h5>${lichXeTuLichHop}<span class="badge badge-success">Đã cấp xe</span></h5>`;
    }
    return "";
}

function thongTinDatXeFormatter(value, row, index) {
    let info = `
    <p><b>Ngày đi: </b> ${row.NgayDiStr} - <b>Thời gian: </b> ${row.ThoiGianBatDauStr} - ${row.ThoiGianKetThucStr ? row.ThoiGianKetThucStr : "..."}</p>
    <p><b>Nơi đi: </b> ${row.NoiDi}</p>
    <p><b>Nơi đến: </b> ${row.NoiDen}</p>
    <p><b>Số người đi: </b> ${row.SoNguoiDi}</p>
    `
    return info;
}

function thongTinLaiXeVaXeFormatter(value, row, index) {
    let info = ``;
    if (row.LaiXe) {
        info += `<p><b>Lái xe: </b> ${row.LaiXe}  </p>
         <p><b>Xe: </b> ${row.Xe}</p>`;
    }

    return info;
}

function thongTinNguoiTaoFormatter(value, row, index) {
    let info = `
    <p><b>Người tạo :</b> ${row.NguoiTao}</p>
    <p><b>Ngày tạo: </b> ${row.NgayTaoStr}</p>
    `
    return info;
}

function tenCuocHopFormatter(value, row, index) {
    let info = row.DieuHanhXeId ? `<i class="fa fa-car"></i>` : ``;

    return row.TenCuocHop + info;
}

function thongTinNguoiDuyetFormatter(value, row, index) {
    return ` ${row.NguoiDuyet ? `<p><b>Người duyệt: </b> </p> ${row.NgayDuyetStr} <p><b>Ngày duyệt: </b> </p>` : ""} `;
}

function thongTinCuocHopFormatter(value, row, index) {
    let info = `
    ${row.NguoiTao ? `<p><b>Người tạo: </b> ${row.NguoiTao}</p>` : ""}
    <p><b>Ngày diễn ra: </b> ${row.NgayDienRaStr} - <b>Thời gian: </b> ${row.BatDauStr} - ${row.KetThucStr ? row.KetThucStr : "..."}</p>
    <p><b>Thành phần tham dự: </b> ${row.NguoiThamGiaKhac || [row.NguoiThamGiaStr, row.NguoiThamGia, row.PhongBanThamGia].filter(Boolean).join('; ') || ""}</p>
    <p><b>Phòng họp: </b> ${row.PhongHopTxt}</p>
    `
    
    if (row.TaiLieuCuocHops && row.TaiLieuCuocHops.length > 0) {
        let taiLieuHtml = '<p><b>File đính kèm: </b>';
        let taiLieuList = [];
        for (let i = 0; i < row.TaiLieuCuocHops.length; i++) {
            let file = row.TaiLieuCuocHops[i];
            // Chỉ hiển thị link tải xuống và nút xem nếu file đã lưu trên server (có DuongDan)
            if (file.DuongDan) {
                let viewBtn = '';
                var filePath = file.DuongDan;
                var extension = filePath.split('.').pop().toLowerCase();
                var allowedViewExtensions = ['doc', 'docx', 'xls', 'xlsx', 'pdf', 'jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg'];
                
                if (allowedViewExtensions.includes(extension)) {
                    viewBtn = `<button onclick="openViewFileModal('${file.DuongDan}', '${file.Name}')" class='btn btn-info btn-xs m-l-5 m-r-5' title='Xem'><i class='fa fa-eye'></i> Xem</button>`;
                }
                
                let downloadBtn = `<button onclick="downloadFromLink('${file.DuongDan}', '${file.Name}')" class='btn btn-primary btn-xs m-l-5 m-r-5' title='Tải xuống'><i class='fa fa-download'></i> Tải xuống</button>`;
                taiLieuList.push(`<span title="${file.Name}"><i class="fa fa-file"></i> ${file.Name} ${viewBtn}${downloadBtn}</span>`);
            } else {
                taiLieuList.push(`<span title="${file.Name}"><i class="fa fa-file"></i> ${file.Name}</span>`);
            }
        }
        taiLieuHtml += taiLieuList.join(', ') + '</p>';
        info += taiLieuHtml;
    }
    
    return info;
}

function thongTinCuocHopTrungFormatter(value, row, index) {
    let info = [];
    if (row.CuocHopTrung) {
        for (let i = 0; i < row.CuocHopTrung.length; i++) {
            let item = row.CuocHopTrung[i];
            info.push(`<a href="javascript:void(0);" onclick="moFormChiTiet(${item.Id})">${item.TenCuocHop}</a>`)
        }
    }
    return info.join(`<div class="dropdown-divider"></div>`);
}

function tenLichXeFormatter(value, row, index) {
    let info = row.IsNgoaiTinh ? `<span class="badge badge-danger ml-3" style="font-size: 95%;">Xe ngoài tỉnh</span>` : ``;

    return row.TieuDe + info;
}

function taoChuoiThoiGian() {
    let now = new Date();
    let hh = String(now.getHours()).padStart(2, '0');
    let mm = String(now.getMinutes()).padStart(2, '0');
    let ss = String(now.getSeconds()).padStart(2, '0');
    let timeStr = hh + mm + ss;

    return parseInt(timeStr, 10);
}

function tenFileFormat(value, row, index) {
    var viewBtn = '';
    var downloadBtn = '';
    var copyBtn = '';
    
    // Chỉ hiển thị nút nếu file đã lưu trên server (có DuongDan)
    if (row.DuongDan) {
        // Nút Copy full URL
        var fullUrl = window.location.origin + row.DuongDan;
        copyBtn = `<button onclick="copyFileUrl('${fullUrl.replace(/'/g, "\\'")}')" class='btn btn-success btn-sm m-l-5 m-r-5 m-t-5 m-b-5' title='Copy đường dẫn file' style='vertical-align: middle;'>
            <i class='fa fa-copy'></i> Copy URL</button>`;
        
        downloadBtn = `<button onclick="downloadFromLink('${row.DuongDan}', '${row.TenFile}')" class='btn btn-primary btn-sm m-l-5 m-r-5 m-t-5 m-b-5' style='vertical-align: middle;'>
            <i class='fa fa-download'></i> Tải xuống</button>`;
        
        var filePath = row.DuongDan;
        var extension = filePath.split('.').pop().toLowerCase();
        var allowedViewExtensions = ['doc', 'docx', 'xls', 'xlsx', 'pdf', 'jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg'];
        
        if (allowedViewExtensions.includes(extension)) {
            viewBtn = `<button onclick="openViewFileModal('${row.DuongDan}', '${row.TenFile}')" class='btn btn-info btn-sm m-l-5 m-r-5 m-t-5 m-b-5' style='vertical-align: middle;'>
                <i class='fa fa-eye'></i> Xem</button>`;
        }
    }
    
    return `<div style='display: flex; align-items: center; flex-wrap: wrap;'><div style='display: inline-flex; align-items: center; margin-right: 10px;'>${copyBtn}${viewBtn}${downloadBtn}</div><span style='vertical-align: middle;'>${row.TenFile}</span></div>`;
}

function copyFileUrl(url) {
    // Tạo một textarea tạm để copy
    var tempTextarea = document.createElement('textarea');
    tempTextarea.value = url;
    tempTextarea.style.position = 'fixed';
    tempTextarea.style.opacity = '0';
    document.body.appendChild(tempTextarea);
    tempTextarea.select();
    
    try {
        document.execCommand('copy');
        toastr.success('Đã copy đường dẫn file vào clipboard!', 'Thành công');
    } catch (err) {
        // Fallback cho trình duyệt không hỗ trợ execCommand
        if (navigator.clipboard && navigator.clipboard.writeText) {
            navigator.clipboard.writeText(url).then(function() {
                toastr.success('Đã copy đường dẫn file vào clipboard!', 'Thành công');
            }, function() {
                toastr.error('Không thể copy đường dẫn file.', 'Lỗi');
            });
        } else {
            toastr.error('Trình duyệt không hỗ trợ copy.', 'Lỗi');
        }
    }
    
    document.body.removeChild(tempTextarea);
}

function getAllowedFileExtensions() {
    return ['.doc', '.docx', '.xls', '.xlsx', '.ppt', '.pdf', '.pptx', '.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp', '.svg', '.txt', '.zip', '.rar'];
}

function isAllowedFileExtension(fileName) {
    if (!fileName) {
        return false;
    }
    var extension = fileName.substring(fileName.lastIndexOf('.')).toLowerCase();
    var allowedExtensions = getAllowedFileExtensions();
    return allowedExtensions.includes(extension);
}

function validateFileExtension(file, errorElementId) {
    if (!file || !file.name) {
        if (errorElementId) {
            $('#' + errorElementId).text('File không hợp lệ.');
        }
        return false;
    }

    if (!isAllowedFileExtension(file.name)) {
        var allowedExtensions = getAllowedFileExtensions().join(', ');
        var errorMsg = `Loại file không được phép. Chỉ chấp nhận các file: ${allowedExtensions}`;
        if (errorElementId) {
            $('#' + errorElementId).text(errorMsg);
        } else {
            toastr.error(errorMsg);
        }
        return false;
    }

    if (errorElementId) {
        $('#' + errorElementId).text('');
    }
    return true;
}

function setVanBanLienQuan(vanBanList, isXem) {
    $('#listVanBan').empty();
    if (!vanBanList || vanBanList.length === 0) return;

    vanBanList.forEach(function (vb) {
        var row = `
            <tr data-vanban-id="${vb.VanBanPhapLuatId}">
                <td>${vb.SoKiHieu || ''} 1</td>
                <td> <a href="javascript:void(0);" onclick="xemChiTietVanBan(${vb.VanBanPhapLuatId})"> ${vb.TrichYeu || ''}
 </a> </td>
                <td>${vb.NgayBanHanhStr || ''}</td>
                <td>${vb.CoQuanBanHanhStr || ''}</td>
                ${isXem ? `` : ` <td>
                    <button class="btn btn-danger btn-sm mr-2 mt-1 mb-1" onclick="removeVanBan(${vb.VanBanPhapLuatId})">
                        <i class="fa fa-trash"></i> 
                    </button>
                   
                </td>`}
               
            </tr>`;
        $('#listVanBan').append(row);
    });
}

function initMainTableAutoHeight(selector, minHeight, paddingBottom) {
    minHeight = minHeight || 350;
    paddingBottom = paddingBottom !== undefined ? paddingBottom : 5;

    function resizeAll() {
        var $tables = selector ? $(selector) : $('.content-wrapper table[data-toggle="table"]').not('.modal table');
        $tables.each(function () {
            var $table = $(this);
            if ($table.is(':visible') && $table.data('bootstrap.table')) {
                var windowHeight = $(window).height();
                var $tableWrapper = $table.closest('.bootstrap-table');
                var offsetTop = $tableWrapper.length && $tableWrapper.offset().top > 0
                    ? $tableWrapper.offset().top
                    : ($table.offset() ? $table.offset().top : 250);
                var footerHeight = $('.main-footer').outerHeight(true) || 50;
                var calculatedHeight = windowHeight - offsetTop - footerHeight - paddingBottom;
                var height = calculatedHeight > minHeight ? calculatedHeight : minHeight;
                $table.bootstrapTable('resetView', {
                    height: height
                });
            }
        });
    }

    resizeAll();
    setTimeout(resizeAll, 200);

    $(window).off('resize.autoTableHeight').on('resize.autoTableHeight', function () {
        resizeAll();
    });

    $(document).off('load-success.bs.table.autoTableHeight').on('load-success.bs.table.autoTableHeight', function (e) {
        if (!$(e.target).closest('.modal').length) {
            resizeAll();
        }
    });
}

