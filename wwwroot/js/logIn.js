document.addEventListener("DOMContentLoaded", () => {
  const loginLink = document.getElementById("login-link");
  const registerLink = document.getElementById("register-link");
  const loginForm = document.getElementById("login-form");
  const registerForm = document.getElementById("register-form");

  loginLink.addEventListener("click", (e) => {
    e.preventDefault();
    loginForm.classList.add("active");
    registerForm.classList.remove("active");
    loginLink.classList.add("active");
    registerLink.classList.remove("active");
  });

  registerLink.addEventListener("click", (e) => {
    e.preventDefault();
    registerForm.classList.add("active");
    loginForm.classList.remove("active");
    registerLink.classList.add("active");
    loginLink.classList.remove("active");
  });
});
