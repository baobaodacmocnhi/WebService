function TimKiem() {
    $("#tblTimKiem tbody tr").remove();
    $.ajax({
        type: 'POST',
        url: '/ZaloChat/getTimKiem/',
        dataType: 'json',
        data: { NoiDungTimKiem: $("#NoiDungTimKiem").val() },
        success: function (data) {
            var items = '';
            $.each(data, function (i, item) {
                var rows = "<tr onClick=getChat('" + item.IDZalo + "')>"
                               + "<td>"
                               + "      <div class='row row-cols-auto'>"
                               + "          <div class='col'><img src='" + item.Avatar + "' width='50' height='50' class='rounded-circle flex-shrink-0'></div>"
                               + "          <div class='col'><b>" + item.Name + "</b></div>"
                               + "          <div class='col'>" + item.CreateDate + "</div>"
                               + "      </div>"
                               + "      <div class='row mt-2'>"
                               + "          <div class='col'>" + item.NoiDung + "</div>"
                               + "      </div>"
                               + "</td>"
                            + "</tr>";
                $("#tblTimKiem tbody").append(rows);
            });
        },
        error: function (ex) {
            alert("Error: " + ex.statusText);
        }
    });
    return false;
}
function getChat(IDZalo) {
    $("#tblChat tbody tr").remove();
    $.ajax({
        type: 'POST',
        url: '/ZaloChat/getChat',
        dataType: 'json',
        data: { IDZalo: IDZalo },
        success: function (data) {
            var items = '';
            $.each(data, function (i, item) {
                var row = "<tr>"
                                     + "<td>"
                                     + "      <div class='row row-cols-auto'>";
                if (item.NguoiGui == 'User') {

                    row += "          <div class='col'><img src='" + item.Avatar + "' width='50' height='50' class='rounded-circle flex-shrink-0'></div>"
                    + "          <div class='col'><b>" + item.Name + "</b></div>";

                }
                else {
                    row += "          <div class='col'><img src='https://service.cskhtanhoa.com.vn/Image/logoctycp.png' width='50' height='50' class='rounded-circle flex-shrink-0'></div>"
                   + "          <div class='col'><b>Cấp Nước Tân Hòa</b></div>";
                }
                row += "          <div class='col'>" + item.CreateDate + "</div>"
                                  + "      </div>"
                                  + "      <div class='row mt-2'>"
                                  + "          <div class='col'>" + item.NoiDung + "</div>"
                                  + "      </div>";
                if (item.Image != "") {
                    row += "      <div class='row'>"
                + "          <div class='col'><a class='example-image-link' href='" + item.Image + "' data-lightbox='example-set' data-title='Click the right half of the image to move forward.'><img class='example-image rounded mx-auto d-block' src='" + item.Image + "' alt='' width='200' height='200'/></a></div>"
                + "      </div>"
                }

                row += "</td>"
             + "</tr>";
                $("#tblChat tbody").append(row);
            });
        },
        error: function (ex) {
            alert("Error: " + ex.statusText);
        }
    });
    return false;
}