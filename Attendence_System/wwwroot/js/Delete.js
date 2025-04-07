$(document).ready(function () {
    $(document).on('click', '.js-delete', function () {
        var btn = $(this);
        var id = btn.data('id');
        var type = btn.data('type');
        var deleteUrl;

        // تحديد الـ URL بناءً على نوع العنصر
        switch (type) {
            case 'student':
                deleteUrl = `/Lecture/DeleteStudent/${id}`;
                break;
            case 'course':
                deleteUrl = `/Manage/DeleteCourse/${id}`;
                break;
            case 'lecture':
                deleteUrl = `/Manage/DeleteLecture/${id}`;
                break;
            default:
                console.error('نوع العنصر غير معروف.');
                return;
        }

        const swal = Swal.mixin({
            customClass: {
                confirmButton: 'btn btn-danger mx-2',
                cancelButton: 'btn btn-light'
            },
            buttonsStyling: false
        });

        swal.fire({
            title: `هل أنت متأكد أنك تريد حذف هذا ${getTypeName(type)}؟`,
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
                    headers: {
                        'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // إضافة التوكن هنا
                    },
                    success: function () {
                        swal.fire(
                            'تم الحذف!',
                            `تم حذف ${getTypeName(type)} بنجاح.`,
                            'success'
                        ).then(() => {
                            // إضافة تأثير عند الحذف
                            btn.closest('tr').fadeOut(300, function () {
                                $(this).addClass('deleted') // إضافة كلاس لتأثير إضافي
                                    .fadeOut(500, function () {
                                        $(this).remove(); // إزالة العنصر بعد التأثير
                                        // عرض رسالة إذا لم يتبق عناصر
                                        if ($('.js-delete[data-type="' + type + '"]').length === 0) {
                                            $('#studentsTable tbody').append(`<tr><td colspan="4" class="text-center">لا يوجد ${getTypeName(type)} مضاف حتى الآن.</td></tr>`);
                                        }
                                    });
                            });
                        });
                    },
                    error: function () {
                        swal.fire(
                            'خطأ!',
                            'حدث خطأ أثناء عملية الحذف.',
                            'error'
                        );
                    }
                });
            }
        });

        function getTypeName(type) {
            switch (type) {
                case 'student':
                    return 'الطالب';
                case 'course':
                    return 'الكورس';
                case 'lecture':
                    return 'المحاضرة';
                default:
                    return '';
            }
        }
    });
});
