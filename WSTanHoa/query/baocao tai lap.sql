select phong.Name,SLnhap=count(distinct tc.ID),SLxuly=count(distinct ls.IDThiCong),SLQuaHan=SUM(CASE WHEN CAST(tc.NgayBatDau as date)<CAST(DATEADD(day,-1,ls.CreateDate) as date) THEN 1 ELSE 0 END)
,SLTon=count(distinct tc.ID)-count(distinct ls.IDThiCong)
from TLMD_Phong phong,TLMD_User users,TLMD_ThiCong tc left join TLMD_ThiCong_LichSu ls on ls.IDThiCong=tc.ID
where tc.CreateBy=users.ID and users.IDPhong=phong.ID
and cast(tc.CreateDate as date)>='20231010' and cast(tc.CreateDate as date)<='20231106'
group by phong.name

select tc.*
from TLMD_Phong phong,TLMD_User users,TLMD_ThiCong tc left join TLMD_ThiCong_LichSu ls on ls.IDThiCong=tc.ID
where tc.CreateBy=users.ID and users.IDPhong=phong.ID and phong.name=N'P. KHĐT'
and tc.ID not in (select IDThiCong from TLMD_ThiCong_LichSu)
and cast(tc.CreateDate as date)>='20230916' and cast(tc.CreateDate as date)<='20231015'


select NgayBatDau,ls.CreateDate from TLMD_ThiCong tc left join TLMD_ThiCong_LichSu ls on ls.IDThiCong=tc.ID
where cast(tc.CreateDate as date)>='20231010' and cast(tc.CreateDate as date)<='20231018'

select name,TenDuong,Phuong=(select TENPHUONG from CAPNUOCTANHOA.dbo.PHUONG where MAQUAN=Quan and MAPHUONG=PHUONG),quan=(select TENQUAN from CAPNUOCTANHOA.dbo.QUAN where MAQUAN=Quan),NgayBatDau from TLMD_ThiCong where cast(CreateDate as date)>='20231010' and cast(CreateDate as date)<='20231106' and id not in
(select distinct tc.ID from TLMD_ThiCong tc,TLMD_ThiCong_LichSu ls
where cast(tc.CreateDate as date)>='20231010' and cast(tc.CreateDate as date)<='20231106'
and tc.ID=ls.IDThiCong) order by quan,phuong