document.addEventListener("DOMContentLoaded", function(){

    document.body.addEventListener('click', function(e) {
        
        var openedLang = document.querySelector('.lang-select.m-open');
        if(openedLang && !e.target.closest('.lang-select.m-open')){
            openedLang.classList.remove('m-open');
        }

        var openedFilter = document.querySelector('.filters__item.m-open');
        if(openedFilter && !e.target.closest('.filters__item.m-open')){
            openedFilter.classList.remove('m-open');
        }

        var openedFilterPanel = document.querySelector('.filters.m-open');
        if(openedFilterPanel && !e.target.closest('.filters.m-open') && !e.target.closest('.btn-filter-trigger')){
            openedFilterPanel.classList.remove('m-open');
        }

    });

    document.body.classList.add('js-ready');
    
    var langSelect = document.querySelectorAll('.lang-select');
    if(langSelect.length){
        [].forEach.call(langSelect, function(elem) {
            elem.addEventListener('click', function(e) {
                var openedLang = document.querySelector('.lang-select.m-open');
                if(openedLang && openedLang !== elem){
                    openedLang.classList.remove('m-open');
                }
                if(e.target.closest('.lang-select__active')){
                    elem.classList.toggle('m-open');
                } else if(e.target.closest('a')){
                    elem.classList.remove('m-open');
                }
            });
        });
    }
    
    var filterSelect = document.querySelectorAll('.filters__item');
    if(filterSelect.length){
        allFiltersCount();
        [].forEach.call(filterSelect, function(elem) {
            var elemN = elem.querySelector('.filters__trigger .e-count');
            var selected = elem.querySelectorAll('.filters__options input:checked').length;
            if(elemN){
                elemN.innerHTML = selected || '';
                elem.querySelector('.filters__trigger').classList[selected ? 'add' : 'remove']('m-active');
            }
            elem.addEventListener('click', function(e) {
                var opened = document.querySelector('.filters__item.m-open');
                if(opened && opened !== elem){
                    opened.classList.remove('m-open');
                }
                if(e.target.closest('.filters__trigger')){
                    if(e.target.closest('.e-icon') && e.target.closest('.filters__trigger.m-active')){
                        // unselect filter
                        var selected = elem.querySelectorAll('.filters__options input:checked');
                        [].forEach.call(selected, function(chk) {
                            chk.checked = false;
                        });
                        if(elemN){ elemN.innerHTML = ''; }
                        elem.querySelector('.filters__trigger').classList.remove('m-active');
                        allFiltersCount();
                    } else {
                        elem.classList.toggle('m-open');
                    }
                } else if(e.target.closest('a')){
                    elem.classList.remove('m-open');
                } else if(e.target.closest('input[type="checkbox"]')){
                    var selected = e.target.closest('.filters__options').querySelectorAll('input:checked').length;
                    if(elemN){
                        elemN.innerHTML = selected || '';
                        elem.querySelector('.filters__trigger').classList[selected ? 'add' : 'remove']('m-active');
                    }
                    allFiltersCount();
                }
            });
        });
    }
    var filtersClear = document.querySelectorAll('.filters .btn-filter-clear');
    if(filtersClear.length){
        [].forEach.call(filtersClear, function(btn) {
            btn.addEventListener('click', function(e) {
                var triggers = btn.closest('.filters').querySelectorAll('.filters__trigger');
                var selected = btn.closest('.filters').querySelectorAll('.filters__options input:checked');
                [].forEach.call(selected, function(chk) {
                    chk.checked = false;
                });
                [].forEach.call(triggers, function(trigger) {
                    var elemN = trigger.querySelector('.e-count');
                    if(elemN) {
                        elemN.innerHTML = '';
                    }
                    trigger.classList.remove('m-active');
                });
                allFiltersCount();
            });
        });
    }
    var filtersPanelClose = document.querySelectorAll('.filters .btn-filter-panel');
    if(filtersPanelClose.length){
        [].forEach.call(filtersPanelClose, function(btn) {
            btn.addEventListener('click', function(e) {
                btn.closest('.filters').classList.remove('m-open');
            });
        });
    }
    var filtersPanelOpen = document.querySelectorAll('.btn-filter-trigger');
    if(filtersPanelOpen.length){
        [].forEach.call(filtersPanelOpen, function(btn) {
            btn.addEventListener('click', function(e) {
                document.querySelector('.filters').classList.add('m-open');
            });
        });
    }
    function allFiltersCount() {
        var selected = document.querySelectorAll('.filters__options input:checked').length;
        var elem = document.querySelector('.filters__controls .e-selected-count .e-n');
        var elem2 = document.querySelector('.btn-filter-trigger .e-n');
        if(elem){
            elem.innerHTML = selected;
        }
        if(elem2){
            elem2.innerHTML = selected || '';
        }
    }

    var headerNavIcon = document.querySelectorAll('.header-nav-icon');
    if(headerNavIcon.length){
        [].forEach.call(headerNavIcon, function(elem) {
            elem.addEventListener('click', function(e) {
                document.body.classList.toggle('m-nav-open');
            });
        });
    }
    
    var sliders = document.querySelectorAll('.cards-slider');
    if(sliders.length){
        [].forEach.call(sliders, function(slider) {
            initSlider2({
                sParent: slider.querySelector('.cards-slider__items > ul'),
                sPager: slider.querySelector('.slider__pager'),
                itemsClass: '.cards-slider__item',
            });
        });
    }
    var sliders2 = document.querySelectorAll('.mcards-slider');
    if(sliders2.length){
        [].forEach.call(sliders2, function(slider) {
            initSlider({
                sParent: slider.querySelector('.cards'),
                sPager: slider.querySelector('.slider__pager'),
                itemsClass: '.cards__item',
                getSlidesVisible: function(){
                    return (window.innerWidth <= 767) ? 1 : 2;
                },
            });
        });
    }
    
    function initSlider(options) {
        var wrapItem = options.sParent;
        var wrapPages = options.sPager;
        var getSlidesVisible = typeof options.getSlidesVisible === 'function' ? options.getSlidesVisible : function(){ return 1; };
        var btnPrev = wrapPages.querySelector('.e-prev');
        var btnNext = wrapPages.querySelector('.e-next');
        if(wrapItem){
            var items = wrapItem.querySelectorAll(options.itemsClass);
            if(!items.length){ return }
            wrapItem.slideActive = 0;
            wrapItem.slideMax = items.length - 1;
    
            if(btnPrev && btnNext){
                sliderCheck();
                setTimeout(function(){ sliderCheck(); }, 500);
                btnPrev.addEventListener('click', function() {
                    if(btnPrev.classList.contains('m-disabled')){ return }
                    var slideWidth = 100 / getSlidesVisible();
                    wrapItem.slideActive = wrapItem.slideActive - 1;
                    wrapItem.style.transform = 'translateX('+(-slideWidth*wrapItem.slideActive + '%)');
                    sliderCheck();
                });
                btnNext.addEventListener('click', function() {
                    if(btnNext.classList.contains('m-disabled')){ return }
                    var slideWidth = 100 / getSlidesVisible();
                    wrapItem.slideActive = wrapItem.slideActive + 1;
                    wrapItem.style.transform = 'translateX('+(-slideWidth*wrapItem.slideActive + '%)');
                    sliderCheck();
                });
            }
    
            function sliderCheck() {
                btnPrev.disabled = wrapItem.slideActive === 0 ? true : false;
                btnNext.disabled = wrapItem.slideActive === (wrapItem.slideMax - getSlidesVisible() + 1) ? true : false;
                [].forEach.call(items, function(item, ind) {
                    item.classList[(ind === wrapItem.slideActive || ind === wrapItem.slideActive + getSlidesVisible() - 1) ? 'add' : 'remove']('m-active');
                    
                });
                sliderHeight();
            }
            function sliderHeight() {
                var item = wrapItem.children[wrapItem.slideActive].querySelector('.e-body');
                var itemCSS = getComputedStyle(item.parentElement);
                var padding = parseInt(itemCSS.paddingTop, 10) + parseInt(itemCSS.paddingBottom, 10);
                if(getSlidesVisible() === 2){
                    var item2 = wrapItem.children[wrapItem.slideActive+1].querySelector('.e-body');
                    if(item && item2 && item.offsetHeight && item2.offsetHeight){
                        wrapItem.style.height = Math.max(item.offsetHeight, item2.offsetHeight) + padding + 'px';
                        return;
                    }
                }
                if(item && item.offsetHeight){
                    wrapItem.style.height = item.offsetHeight + padding + 'px';
                }
            }
            window.addEventListener('resize', debounce(function() {
                if(wrapItem.children[wrapItem.slideActive].getBoundingClientRect().x < 0){
                    btnPrev.click();
                    return;
                }
                sliderCheck()
            }, 300));
        }
    }
    
    function initSlider2(options) {
        var wrapItem = options.sParent;
        var wrapPages = options.sPager;
        var btnPrev = wrapPages.querySelector('.e-prev');
        var btnNext = wrapPages.querySelector('.e-next');
        if(wrapItem){
            var items = wrapItem.querySelectorAll(options.itemsClass);
            if(!items.length){ return }
            wrapItem.slideActive = 0;
            wrapItem.slideMax = items.length - 1;
    
            if(btnPrev && btnNext){
                wrapItem.children[wrapItem.slideActive].classList.add('m-active');
                sliderCheck();
                setTimeout(function(){ sliderCheck(); }, 500);
                btnPrev.addEventListener('click', function() {
                    if(btnPrev.classList.contains('m-disabled')){ return }
                    var currentSlide = wrapItem.children[wrapItem.slideActive];
                    var nextSlide = wrapItem.children[wrapItem.slideActive - 1];
                    nextSlide.classList.add('m-prev');
                    currentSlide.classList.add('m-active-hide')
                    wrapItem.slideActive = wrapItem.slideActive - 1;
                    sliderCheck();
                    setTimeout(function(){
                        nextSlide.classList.add('m-prev-move');
                    }, 10);
                    setTimeout(function(){
                        nextSlide.classList.add('m-active');
                        nextSlide.classList.remove('m-prev', 'm-prev-move');
                        currentSlide.classList.remove('m-active', 'm-active-hide');
                    }, 600);
                });
                btnNext.addEventListener('click', function() {
                    if(btnNext.classList.contains('m-disabled')){ return }
                    var currentSlide = wrapItem.children[wrapItem.slideActive];
                    var nextSlide = wrapItem.children[wrapItem.slideActive + 1];
                    nextSlide.classList.add('m-next');
                    currentSlide.classList.add('m-active-hide')
                    wrapItem.slideActive = wrapItem.slideActive + 1;
                    sliderCheck();
                    setTimeout(function(){
                        nextSlide.classList.add('m-next-move');
                    }, 10);
                    setTimeout(function(){
                        nextSlide.classList.add('m-active');
                        nextSlide.classList.remove('m-next', 'm-next-move');
                        currentSlide.classList.remove('m-active', 'm-active-hide');
                    }, 600);
                });
            }
    
            function sliderCheck() {
                btnPrev.disabled = wrapItem.slideActive === 0 ? true : false;
                btnNext.disabled = wrapItem.slideActive === wrapItem.slideMax ? true : false;
                sliderHeight();
            }
            function sliderHeight() {
                var item = wrapItem.children[wrapItem.slideActive].querySelector('.e-body');
                var itemCSS = getComputedStyle(item.parentElement);
                var padding = parseInt(itemCSS.paddingTop, 10) + parseInt(itemCSS.paddingBottom, 10);
                if(item && item.offsetHeight){
                    wrapItem.style.height = item.offsetHeight + padding + 'px';
                }
            }
            window.addEventListener('resize', debounce(function() {
                sliderCheck();
            }, 300));
        }
    }

    var cardsItems = document.querySelectorAll('.cards .cards__item');
    if(cardsItems.length){
        [].forEach.call(cardsItems, function(item) {
            var pos = item.getBoundingClientRect().top;
            if(pos >= window.outerHeight){
                item.classList.add('u-fade');
                item.classList.add('u-fade-a');
                item.classList.add('u-fade-out');
            }
        });
        window.addEventListener('scroll', function(e) {
            var items = document.querySelectorAll('.cards .cards__item.u-fade-out');
            if(items.length){
                [].forEach.call(items, function(item) {
                    var pos = item.getBoundingClientRect().top;
                    if(pos < window.outerHeight){
                        item.classList.remove('u-fade-out');
                        item.classList.add('u-fade-out2');
                        setTimeout(function(){
                            item.classList.remove('u-fade-out2');
                        }, 300);
                        setTimeout(function(){
                            item.classList.remove('u-fade-a');
                        }, 1000);
                    }
                });
            }
        });
    }

    var formFields = document.querySelectorAll('.c-form__field');
    if(formFields.length){
        [].forEach.call(formFields, function(field) {
            var input = field.querySelector('.c-form__input') || field.querySelector('.c-form__tarea');
            if(input){
                input.addEventListener('focus', function(){
                    field.classList.add('m-focus');
                });
                input.addEventListener('blur', function(){
                    if(!input.value){
                        field.classList.remove('m-focus');
                    }
                });
                var inputCount = field.querySelector('.c-form__tarea-count');
                if(inputCount){
                    var inputCountMax = inputCount.getAttribute('data-max')*1 || 100;
                    inputCount.innerHTML = input.value.length + ' / ' + inputCountMax;
                    input.addEventListener('input', function(){
                        inputCount.innerHTML = input.value.length + ' / ' + inputCountMax;
                        field.classList[input.value.length > inputCountMax ? 'add' : 'remove']('m-error');
                    });
                }
            }
        });
    }
    var formFiles = document.querySelectorAll('.c-form__filefield');
    if(formFiles.length){
        [].forEach.call(formFiles, function(field) {
            field.addEventListener('drop', function(e){
                e.stopPropagation();
                e.preventDefault();
                field.querySelector('.c-form__filefield-input').files = e.dataTransfer.files;
            });
        });
    }

    var sBottomClose = document.querySelector('.section-bottom .btn-close');
    if(sBottomClose){
        sBottomClose.addEventListener('click', function(){
            document.querySelector('.section-bottom').classList.remove('m-open');
        });
    }

});


if (!Element.prototype.matches) {
    Element.prototype.matches = Element.prototype.msMatchesSelector || Element.prototype.webkitMatchesSelector;
}
if (!Element.prototype.closest) {
    Element.prototype.closest = function(s) {
        var el = this;
    
        do {
            if (el.matches(s)) return el;
            el = el.parentElement || el.parentNode;
        } while (el !== null && el.nodeType === 1);
        return null;
    };
}

function debounce(func, wait, immediate) {
    var timeout;
    return function() {
        var context = this, args = arguments;
        var later = function() {
            timeout = null;
            if (!immediate) func.apply(context, args);
        };
        var callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func.apply(context, args);
    }
}

let cookieBtns = document.querySelectorAll('.section-bottom__btns button');

if (cookieBtns) {
    if (getCookie('acceptCookie')==='true') {
        cookieBtns.forEach(function(button) {
            button.closest('.section-bottom').classList.remove('m-open');
        });
    }
    cookieBtns.forEach(function(button) {
        button.onclick = function(click) {
            let cookieBlock = click.target.closest('.section-bottom');
            document.cookie = 'acceptCookie=true';
            cookieBlock.classList.remove('m-open');
        };
    });
}

