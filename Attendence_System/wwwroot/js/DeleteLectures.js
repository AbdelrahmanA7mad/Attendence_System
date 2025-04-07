$(document).ready(function () {
    $('#deleteAllLecturesBtn').on('click', function () {
        const deleteUrl = '/Lecture/DeleteAllLectures'; // Adjust URL if necessary

        Swal.fire({
            title: 'هل أنت متأكد أنك تريد حذف جميع المحاضرات؟',
            text: "لن تتمكن من التراجع عن هذا الإجراء!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'نعم، احذفه!',
            cancelButtonText: 'لا، إلغاء!',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: deleteUrl,
                    method: 'DELETE',
                    success: function () {
                        Swal.fire(
                            'تم الحذف!',
                            'تم حذف جميع المحاضرات بنجاح.',
                            'success'
                        ).then(() => {
                            // Optionally, reload or update the page dynamically
                        });
                    },
                    error: function () {
                        Swal.fire(
                            'خطأ!',
                            'حدث خطأ أثناء عملية الحذف.',
                            'error'
                        );
                    }
                });
            }
        });
    });
});
