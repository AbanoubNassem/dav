import "./plugin.scss";
import LoginPage from "@/login_page";

window.trinityApp.serving((app) => {
  app.registerPage("Login", LoginPage, LoginPage.layout);
});
