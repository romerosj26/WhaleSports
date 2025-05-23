document.addEventListener('DOMContentLoaded', () => {
  const langs = document.querySelector('.langs');
  const selected = langs.querySelector('.langs-selected-lang');
  const dropdown = langs.querySelector('.langs-dropdown');
  const links = langs.querySelectorAll('a[data-lang]');
  const flagImg = document.getElementById('lang-flag');
  let lang = localStorage.getItem('lang') || 'english';
  let index = parseInt(localStorage.getItem('langIndex'), 10) || 0;

      // Validar índice
    if (index < 0 || index >= links.length) {
      index = 0;
      localStorage.setItem('langIndex', index);
    }

  // Inicializar estado visual
  links.forEach(l => l.classList.remove('active'));
  if (links[index]) links[index].classList.add('active');
  updateSelectedLangDisplay(lang, links[index]);
  changeLang(lang);

  // Abrir/cerrar el menú al hacer clic
  selected.addEventListener('click', () => {
    langs.classList.toggle('open');
  });

  // Cerrar si se hace clic fuera
  document.addEventListener('click', (e) => {
    if (!langs.contains(e.target)) {
      langs.classList.remove('open');
    }
  });

  // Click handler
  links.forEach((el, i) => el.addEventListener('click', e => {
    e.preventDefault();
    langs.querySelector('a.active')?.classList.remove('active');

    el.classList.add('active');
    lang = el.dataset.lang;
    index = i;
    localStorage.setItem('lang', lang);
    localStorage.setItem('langIndex', index);
    
    updateSelectedLangDisplay(lang, el);
    changeLang(lang);
    langs.classList.remove('open'); // Cierra menú
  }));

  function updateSelectedLangDisplay(lang, linkEL) {
  const label = document.getElementById('lang-label');
  if (label) label.textContent = linkEL.textContent;

  if (flagImg && linkEL && linkEL.dataset.flag) {
    flagImg.src = linkEL.dataset.flag;
    flagImg.alt = lang;
  }
}
  async function changeLang(lang) {
    try {
      const res = await fetch(`/i18n/${lang}.json`);
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      const data = await res.json();

      for (const key in data) {
        const el = document.querySelector(`.lng-${key}`);
        if (el) {
            if(el.tagName ==="INPUT" && el.hasAttribute("placeholder")){
                el.setAttribute("placeholder", data[key]);
            }else{
                 el.textContent = data[key];
            }
          }
        } 
      }catch (err) {
        console.error(`Error cargando idioma "${lang}":`, err);
      }
    }
  });