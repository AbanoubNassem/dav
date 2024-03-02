import { useEffect, useState } from "react";

import { Button } from "primereact/button";
import { Checkbox } from "primereact/checkbox";
import { classNames } from "primereact/utils";
import { PrimeReactProvider } from "primereact/api";
import { Head, useForm, usePage } from "@inertiajs/react";
import { Message } from "primereact/message";

const Login = () => {
  const configs = window.trinityApp.configs;
  const { errors: pErrors, data: responseData, pages } = usePage().props;

  let errors = pErrors ?? {};

  const [password, setPassword] = useState("");
  const [checked, setChecked] = useState(false);
  const localize = window.trinityApp.localize;
  const containerClassName = classNames(
    "surface-ground flex align-items-center justify-content-center min-h-screen min-w-screen overflow-hidden p-input-filled",
  );

  const { data, setData, post, processing } = useForm({
    email: "",
    password: "",
    remember: false,
  });

  useEffect(() => {
    if (responseData) {
      location.href = String(responseData);
    }
  }, [responseData]);

  return (
    <>
      <title>Login</title>
      <div className={containerClassName}>
        <div className="flex flex-column align-items-center justify-content-center">
          <img
            src={`${configs?.prefix}/trinity/dist/logo.svg`}
            alt={configs.title}
            className="mb-5 w-6rem flex-shrink-0"
          />
          <div
            style={{
              borderRadius: "56px",
              padding: "0.3rem",
              background:
                "linear-gradient(180deg, var(--primary-color) 10%, rgba(33, 150, 243, 0) 30%)",
            }}
          >
            <form
              className="w-full surface-card py-8 px-5 sm:px-8"
              style={{ borderRadius: "53px" }}
              onSubmit={(e) => {
                e.preventDefault();
                post(``);
              }}
            >
              <div
                className="p-message p-component p-message-info p-message-enter-done"
                role="alert"
                aria-live="assertive"
                aria-atomic="true"
                data-pc-name="messages"
                data-pc-section="root"
              >
                <div className="p-message-wrapper" data-pc-section="wrapper">
                  <svg
                    width="14"
                    height="14"
                    viewBox="0 0 14 14"
                    fill="none"
                    xmlns="http://www.w3.org/2000/svg"
                    className="p-icon p-message-icon"
                    aria-hidden="true"
                    data-pc-section="icon"
                  >
                    <path
                      clipRule="evenodd"
                      d="M3.11101 12.8203C4.26215 13.5895 5.61553 14 7 14C8.85652 14 10.637 13.2625 11.9497 11.9497C13.2625 10.637 14 8.85652 14 7C14 5.61553 13.5895 4.26215 12.8203 3.11101C12.0511 1.95987 10.9579 1.06266 9.67879 0.532846C8.3997 0.00303296 6.99224 -0.13559 5.63437 0.134506C4.2765 0.404603 3.02922 1.07129 2.05026 2.05026C1.07129 3.02922 0.404603 4.2765 0.134506 5.63437C-0.13559 6.99224 0.00303296 8.3997 0.532846 9.67879C1.06266 10.9579 1.95987 12.0511 3.11101 12.8203ZM3.75918 2.14976C4.71846 1.50879 5.84628 1.16667 7 1.16667C8.5471 1.16667 10.0308 1.78125 11.1248 2.87521C12.2188 3.96918 12.8333 5.45291 12.8333 7C12.8333 8.15373 12.4912 9.28154 11.8502 10.2408C11.2093 11.2001 10.2982 11.9478 9.23232 12.3893C8.16642 12.8308 6.99353 12.9463 5.86198 12.7212C4.73042 12.4962 3.69102 11.9406 2.87521 11.1248C2.05941 10.309 1.50384 9.26958 1.27876 8.13803C1.05367 7.00647 1.16919 5.83358 1.61071 4.76768C2.05222 3.70178 2.79989 2.79074 3.75918 2.14976ZM7.00002 4.8611C6.84594 4.85908 6.69873 4.79698 6.58977 4.68801C6.48081 4.57905 6.4187 4.43185 6.41669 4.27776V3.88888C6.41669 3.73417 6.47815 3.58579 6.58754 3.4764C6.69694 3.367 6.84531 3.30554 7.00002 3.30554C7.15473 3.30554 7.3031 3.367 7.4125 3.4764C7.52189 3.58579 7.58335 3.73417 7.58335 3.88888V4.27776C7.58134 4.43185 7.51923 4.57905 7.41027 4.68801C7.30131 4.79698 7.1541 4.85908 7.00002 4.8611ZM7.00002 10.6945C6.84594 10.6925 6.69873 10.6304 6.58977 10.5214C6.48081 10.4124 6.4187 10.2652 6.41669 10.1111V6.22225C6.41669 6.06754 6.47815 5.91917 6.58754 5.80977C6.69694 5.70037 6.84531 5.63892 7.00002 5.63892C7.15473 5.63892 7.3031 5.70037 7.4125 5.80977C7.52189 5.91917 7.58335 6.06754 7.58335 6.22225V10.1111C7.58134 10.2652 7.51923 10.4124 7.41027 10.5214C7.30131 10.6304 7.1541 10.6925 7.00002 10.6945Z"
                      fill="currentColor"
                    ></path>
                  </svg>
                  <span
                    className="p-message-summary "
                    data-pc-section="summary"
                    style={{ fontWeight: 700, marginRight: 10 }}
                  >
                    Info
                  </span>
                  <span className="p-message-detail" data-pc-section="detail">
                    If you don't have account , by logging in , we will create
                    one for you.
                  </span>
                </div>
              </div>
              <div>
                <div className="field">
                  <label
                    htmlFor="email1"
                    className="block text-900 text-xl font-medium mb-2"
                  >
                    {localize("email")}
                  </label>
                  <input
                    type="email"
                    required
                    placeholder={localize("email_address")}
                    className={classNames(
                      "p-inputtext p-component w-full mb-3",
                      {
                        "p-invalid": errors.email,
                      },
                    )}
                    value={data.email}
                    onChange={(e) => setData("email", e.target.value)}
                    style={{ padding: "1rem" }}
                  />
                  {errors.email && (
                    <small className="p-error w-full block md:w-30rem">
                      {errors.email}
                    </small>
                  )}
                </div>
                <div className="field">
                  <label className="block text-900 font-medium text-xl mb-2">
                    {localize("Wallet Address")}
                  </label>
                  <input
                    value={password}
                    onChange={(e) => {
                      setPassword(e.target.value);
                      setData("password", e.target.value);
                    }}
                    placeholder={localize("Wallet Address")}
                    type="password"
                    required
                    className={classNames(
                      "p-inputtext p-component p-password-input w-full p-3 ",
                      {
                        "p-invalid": errors.password,
                      },
                    )}
                  />
                  {errors.password && (
                    <small className="p-error w-full block md:w-30rem">
                      {errors.password}
                    </small>
                  )}
                </div>
                <div className="flex align-items-center justify-content-between mb-5 gap-5">
                  <div className="flex align-items-center">
                    <Checkbox
                      checked={checked}
                      onChange={(e) => {
                        setChecked(e.checked ?? false);
                        setData("remember", e.checked ?? false);
                      }}
                      className="mr-2"
                    />
                    <label htmlFor="rememberme1">
                      {localize("remember_me")}
                    </label>
                  </div>
                </div>

                {errors.login && (
                  <Message
                    severity="error"
                    text={errors.login}
                    className="mb-3"
                  />
                )}

                <Button
                  label={localize("sign_in")}
                  className="w-full p-3 text-xl"
                  type="submit"
                  loading={processing}
                  disabled={processing}
                />
              </div>
            </form>
          </div>
        </div>
      </div>
    </>
  );
};

Login.layout = null as any;
export default Login;
