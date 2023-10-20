select phong.Name,SLnhap=count(tc.ID),SLxuly=count(ls.ID),SLQuaHang=SUM(CASE WHEN CAST(tc.NgayBatDau as date)<CAST(DATEADD(day,-1,ls.CreateDate) as date) THEN 1 ELSE 0 END)
from TLMD_Phong phong,TLMD_User users,TLMD_ThiCong tc left join TLMD_ThiCong_LichSu ls on ls.IDThiCong=tc.ID
where tc.CreateBy=users.ID and users.IDPhong=phong.ID
and cast(tc.CreateDate as date)>='20230916' and cast(tc.CreateDate as date)<='20231015'
group by phong.name

select tc.*
from TLMD_Phong phong,TLMD_User users,TLMD_ThiCong tc left join TLMD_ThiCong_LichSu ls on ls.IDThiCong=tc.ID
where tc.CreateBy=users.ID and users.IDPhong=phong.ID and phong.name=N'P. KHĐT'
and tc.ID not in (select IDThiCong from TLMD_ThiCong_LichSu)
and cast(tc.CreateDate as date)>='20230916' and cast(tc.CreateDate as date)<='20231015'


select NgayBatDau,ls.CreateDate from TLMD_ThiCong tc left join TLMD_ThiCong_LichSu ls on ls.IDThiCong=tc.ID
where cast(tc.CreateDate as date)>='20231010' and cast(tc.CreateDate as date)<='20231018'