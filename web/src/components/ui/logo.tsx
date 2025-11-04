import { toAbsoluteUrl } from '@/lib/helpers';
import { useWhiteLabel } from '@/hooks/use-white-label';

export function Logo() {
  const theme = useWhiteLabel();

  return (
    <>
      <div className="dark:hidden">
        <img
          src={toAbsoluteUrl(theme.assets.logo)}
          className="default-logo h-[22px] max-w-none"
          alt={`Logo da ${theme.name}`}
        />
        <img
          src={toAbsoluteUrl(theme.assets.logoMini)}
          className="small-logo h-[22px] max-w-none"
          alt={`Logo da ${theme.name}`}
        />
      </div>
      <div className="hidden dark:block">
        <img
          src={toAbsoluteUrl(theme.assets.logo)}
          className="default-logo h-[22px] max-w-none"
          alt={`Logo da ${theme.name}`}
        />
        <img
          src={toAbsoluteUrl(theme.assets.logoMini)}
          className="small-logo h-[22px] max-w-none"
          alt={`Logo da ${theme.name}`}
        />
      </div>
    </>
  );
}
