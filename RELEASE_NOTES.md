# GeniaClipboard 0.5.1 — UI & Usability

[Русский ниже](#русский)

## English

A focused usability update for the 0.5 Privacy Core.

**What's improved**
- Settings are organized into four tabs: Security, History, Hotkeys, and System (Russian labels in the current interface).
- Save and Cancel stay visible in a fixed footer while the contents of each tab scroll independently.
- Vault selection uses shorter, readable option names with explanations below.
- The excluded-process list now uses the full available width.
- The unencrypted TXT-journal warning is fully visible in the System tab.
- Settings labels no longer overwrite the clipboard on double-click.
- Invalid hotkey or vault-password inputs return you to the relevant tab.
- Layout scales more predictably when Windows display scaling is increased.

**Compatibility**

All existing 0.5.0 privacy and vault features remain available. The encrypted history format and saved settings schema are unchanged; version 0.5.1 is intended as an in-place upgrade.

**Security reminder:** local encrypted history protects data at rest, not clipboard or process memory from malware already running as your user. TXT exports and the optional journal are still plaintext.

## Русский

Это обновление удобства для ветки Privacy Core 0.5.

**Что улучшено**
- Настройки разделены на четыре вкладки: **Безопасность**, **История**, **Горячие клавиши**, **Система**.
- Кнопки «Сохранить» и «Отмена» постоянно видны внизу; прокрутка выполняется внутри вкладок.
- Варианты Vault больше не обрезаются: короткие названия и отдельные пояснения.
- Поле исключённых процессов использует всю доступную ширину.
- Предупреждение о незашифрованном TXT-журнале не скрывается под кнопками.
- Двойной клик по подписям настроек не изменяет буфер обмена.
- При ошибке в хоткее или мастер-пароле автоматически открывается нужная вкладка.
- Улучшено поведение интерфейса при масштабировании Windows.

**Совместимость**

Все возможности Privacy Core из 0.5.0 сохраняются. Формат зашифрованной истории и структура сохранённых настроек не изменены. Версию 0.5.1 можно устанавливать поверх 0.5.0.

**Напоминание:** encrypted vault защищает историю на диске, но не системный буфер и память от malware текущего пользователя. TXT-журналы и ручной экспорт по-прежнему не шифруются.
