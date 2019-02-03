export function removeTags(text) {
  if (text != null) {
    return text.replace(/<\/?[^>]+(>|$)/g, "");
  }
}
