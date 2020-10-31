import Swal from "sweetalert2";
import withReactContent from "sweetalert2-react-content";
const MySwal = withReactContent(Swal);

export function getCookie(cname) {
  var name = cname + "=";
  var decodedCookie = decodeURIComponent(document.cookie);
  var ca = decodedCookie.split(";");
  for (var i = 0; i < ca.length; i++) {
    var c = ca[i];
    while (c.charAt(0) == " ") {
      c = c.substring(1);
    }
    if (c.indexOf(name) == 0) {
      return c.substring(name.length, c.length);
    }
  }
  return "";
}
export function viewError(title, msg) {
  MySwal.fire({
    icon: "error",
    title: title,
    text: msg,
  });
}
export function viewSuccess(title, msg) {
  MySwal.fire({
    icon: "success",
    title: title,
    text: msg,
  });
}
export function testEmail(mail) {
  const emailRegex = /^(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;
  if (!emailRegex.test(mail)) {
    return false;
  }
  return true;
}
export async function removeConfirmation() {
  return await MySwal.fire({
    title: "Are you sure?",
    text: "You won't be able to revert this!",
    icon: "warning",
    showCancelButton: true,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, delete it!",
  });
}
export function formatDate(date) {
  const dateObj = new Date(date);
  //Convert the date from UTC to local time zone.
  dateObj.setMinutes(dateObj.getMinutes() - dateObj.getTimezoneOffset());
  return `${dateObj.getFullYear()}-${("0" + dateObj.getMonth()).slice(-2)}-${(
    "0" + dateObj.getDate()
  ).slice(-2)} ${("0" + dateObj.getHours()).slice(-2)}:${(
    "0" + dateObj.getMinutes()
  ).slice(-2)}
  `;
}

export function checkSession() {
  fetch("api/users/checksession").then((response) => {
    if (response.status == 401) {
      const queryStringIndex = window.location.href.indexOf("?");
      sessionStorage.setItem("requestedUrl", window.location.pathname);
      document.getElementById("loginlink").click();
    }
  });
}
