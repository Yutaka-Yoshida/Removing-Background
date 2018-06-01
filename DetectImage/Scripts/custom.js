
function readURL(input) {
    $('.face').remove();
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#picture').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}
jQuery(function ($) {
    $("#imgInp").val("");
    $("#imgInp").change(function () {
        readURL(this);
    });

    $("#check-btn").click(function () {
        var selectedFile = ($("#imgInp"))[0].files[0];//FileControl.files[0];        
        var form = $('#imgInp')[0];
        if (selectedFile) {
            var dataString = new FormData();
            dataString.append('file', selectedFile);
            $.ajax({
                url: siteUrl + "/Home/CheckImage",  //Server script to process data
                type: 'POST',
                success: function(response){
                    if (response.status == true) alert("Success");
                    else alert("Fail");

                    $("#download-url").attr("href", siteUrl +"/Images/"+ response.url);
                    $("#download-url").show();
                },
                error: function (error) {
                    console.log(error);
                },
                complete: function () {
                },
                data: dataString,
                dataType: 'json',
                cache: false,
                contentType: false,
                processData: false
            });

        }
        else {
            alert("Please select file");
        }
    });
});
