function showVideo(elemId, embedCode)
{
    var elem = document.getElementById(elemId);
    var video = document.createElement("div");
    video.innerHTML = embedCode;
    
    elem.parentNode.replaceChild(video, elem);
}
