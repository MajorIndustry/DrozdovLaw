// links pages
document.addEventListener("DOMContentLoaded", function(){
    var html = '<a id="demo-pages-nav-close" href="javascript:void(0);" onclick="this.parentElement.outerHTML = \'\'">Закрыть</a> \
    <style> \
        #demo-pages-nav {position: fixed; z-index: 1005; bottom: 0; right: 0; background: #fff; border: solid 1px #ccc; border-bottom: none; border-right: none; width: 260px; opacity: .4; transition: opacity .3s linear;} \
        #demo-pages-nav:hover {opacity: 1; } \
        #demo-pages-nav-close {float: right;background:#ccc; color:#000; padding: 5px 10px; text-decoration: none; font-size: 14px;} \
        #demo-pages-list { padding: 30px 20px 10px 40px; font-size: 14px; } \
        #demo-pages-list a { text-decoration: none; color:#000; } \
        #demo-pages-list li { margin: 5px 0; } \
    </style> \
    <ol id="demo-pages-list"> \
        <li><a href="index.html">Главная</a></li> \
        <li><a href="cases.html">Cases</a></li> \
        <li><a href="case.html">Case</a></li> \
        <li><a href="tag.html">Tag</a></li> \
        <li><a href="services.html">Services</a></li> \
        <li><a href="service.html">Service</a></li> \
        <li><a href="vacancies.html">Vacancies</a></li> \
        <li><a href="feedback.html">Feedback</a></li> \
        <li><a href="whoweare.html">Who we are</a></li> \
        <li><a href="contacts.html">Contacts</a></li> \
        <li><a href="404.html">404</a></li> \
    </ol>';

    var wrap = document.createElement('div');
    wrap.id = 'demo-pages-nav';
    wrap.innerHTML = html;
    document.body.appendChild(wrap);
});