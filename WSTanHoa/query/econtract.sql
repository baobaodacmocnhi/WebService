--select * from Zalo_EContract_ChiTiet where shs not like ''

SELECT shs,COTLK,DHN_SODANHBO,DHN_SOHOPDONG,DHN_NGAYCHOSODB=CONVERT(varchar(10),DHN_NGAYCHOSODB,103)
FROM TANHOA_WATER.dbo.KH_HOSOKHACHHANG where shs in (select SHS from Zalo_EContract_ChiTiet where shs not like '' and hieuluc=0) and DHN_SODANHBO is not null

--select b.DanhBo,c.MaDonMoi,b.CreateDate from Zalo_EContract_ChiTiet a,KTKS_DonKH.dbo.DCBD_ChiTietBienDong b,KTKS_DonKH.dbo.DCBD c
--where a.MaDon=c.MaDonMoi and b.MaDCBD=c.MaDCBD and b.ThongTin like N'%T�n%'
--and a.HieuLuc=0 and CAST(a.CreateDate as date)>='20231012' order by c.MaDonMoi


