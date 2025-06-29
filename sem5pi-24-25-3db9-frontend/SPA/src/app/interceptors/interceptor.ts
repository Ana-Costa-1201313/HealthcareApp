import { HttpHandlerFn, HttpInterceptorFn, HttpRequest } from "@angular/common/http";

//export const authenticationInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next:
//    HttpHandlerFn) => {
//    const modifiedReq = req.clone({
//        headers: req.headers.set('Authorization', `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhZG1pbkBob3RtYWlsLmNvbSIsInJvbGUiOiJBZG1pbiIsImp0aSI6ImQ3MWUwMGFlLTcwNjYtNDEzNi04M2E1LTIwMzRmNTk1ZGRkYyIsImlhdCI6MTczMDU3MTY5MSwibmJmIjoxNzMwNTcxNjkxLCJleHAiOjE3MzA4MzA4OTEsImlzcyI6IkF1dGhfQmFja09mZmljIiwiYXVkIjoiQVVUSCJ9.YFGc-vCCLvzT12mvE7Xa7UkZlpWN1-i2u_sTgJl5KbQ`),
//    });

//    return next(modifiedReq);
//};

export const authenticationInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next:
  HttpHandlerFn) => {

  // Function to get the JWT token from cookies
  const getCookie = (name: string): string | undefined => {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop()!.split(';').shift();
    return undefined;
  };

  // Get  JWT token cookie
  const token = getCookie('jwt');

  // token exists, clone request add Authorization header
  if (token) {
    const modifiedReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json; charset=utf-8',
      },
    });
    return next(modifiedReq); // Send request with token
  }

  return next(req); // If no token, send the request as is
};
