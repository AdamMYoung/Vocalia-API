/**
 * Returns true if the current device is a mobile device, false if not.
 */
export function isMobile(): boolean {
  if (
    /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(
      navigator.userAgent
    )
  ) {
    return true;
  }
  return false;
}
