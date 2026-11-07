UPDATE VaiTro
SET PhanQuyenKeys = PhanQuyenKeys + ',KhoNhiemVuHoanThanh'
WHERE PhanQuyenKeys LIKE '%GiaoViec%' AND PhanQuyenKeys NOT LIKE '%KhoNhiemVuHoanThanh%';

INSERT INTO DatabaseVersion(Name) VALUES(N'20261107111128');

