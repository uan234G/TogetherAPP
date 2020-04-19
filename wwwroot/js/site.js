const pwdInput = document.getElementById('password');
const show = document.getElementById('show-pwd');

const mobileDashNav = document.querySelector('.mobile-dashNav');
const dashNavBtn = document.getElementById('dash-btn');

const mobileNav = document.querySelector('.mobile-nav');
const navBtn = document.getElementById('nav-btn');

const menuIcon = document.getElementById('menu-icon');

function showPwd() {
    if (pwdInput.type === 'password'){
        show.innerHTML = '<i class="far fa-eye-slash"></i>';
        pwdInput.type = 'text';
    } else{
        show.innerHTML = '<i class="far fa-eye"></i>';
        pwdInput.type = 'password';
    }
}

navBtn.addEventListener('click', landingNav);
dashNavBtn.addEventListener('click', dashNav);

function landingNav(){
    console.log('clicked');
    menuIcon.classList.toggle('animate')
    mobileNav.classList.toggle('show');
}

function dashNav(){
    console.log('clicked dash');
    menuIcon.classList.toggle('animate')
    mobileDashNav.classList.toggle('show');
}