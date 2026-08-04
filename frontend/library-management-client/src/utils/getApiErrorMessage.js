export default function getApiErrorMessage(
  error,
  fallbackMessage = "İşlem sırasında bir hata oluştu.",
) {
  const responseData = error?.response?.data;

  if (typeof responseData?.message === "string") {
    return responseData.message;
  }

  if (responseData?.errors) {
    const validationMessages = Object.values(
      responseData.errors,
    ).flat();

    if (validationMessages.length > 0) {
      return validationMessages.join(" ");
    }
  }

  if (error?.code === "ECONNABORTED") {
    return "Sunucu yanıt vermedi. Lütfen tekrar deneyin.";
  }

  if (!error?.response) {
    return "Backend sunucusuna bağlanılamadı.";
  }

  return fallbackMessage;
}