import { HttpHandlerFn, HttpInterceptorFn, HttpRequest } from "@angular/common/http";

export const authenticationInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next:
    HttpHandlerFn) => {
    const modifiedReq = req.clone({
        headers: req.headers.set('Authorization', `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhZG1pbkBob3RtYWlsLmNvbSIsInJvbGUiOiJBZG1pbiIsImp0aSI6IjVmOTM2N2IzLWU3ODYtNGJmOS05MzFkLWMyMTI4ZDhmODFlMCIsImlhdCI6MTczMjI3MzMxMSwibmJmIjoxNzMyMjczMzExLCJleHAiOjQ4ODc5NDY5MTEsImlzcyI6IkF1dGhfQmFja09mZmljIiwiYXVkIjoiQVVUSCJ9.6ArzE5N5W4Xigqix7bLa2yB0YCHEAe5papueKkEbiTo`),
    });

    return next(modifiedReq);
};