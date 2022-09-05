﻿

var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "Product/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "description", "width": "15%" },
            { "data": "customerPrice", "width": "15%" },
            { "data": "resellerPrice", "width": "15%" },
            { "data": "category.name", "width": "15%" },
            { "data": "brand.name", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Product/Upsert?id=${data}"
                        class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Update</a>
                        <a onClick=Delete('/Product/Delete/${data}')
                        class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
					</div>
                        `
                },
                "width": "15%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: '',
        text: "Are you sure you want to delete this?",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Confirm'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        location.reload();
                        toastr.success(data.message);
                        
                        
                    }
                    else {
                        location.reload();
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}
