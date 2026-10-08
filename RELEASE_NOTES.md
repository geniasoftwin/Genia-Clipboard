# GeniaClipboard 0.5.4 — Search Alignment

[Русский ниже](#русский)

## English

A small UI patch following Windows user testing of 0.5.3.

- Restores the full-height search field.
- Centers both placeholder and entered search text vertically in the search field.
- Uses a native Windows panel border around a natural-height, borderless text input to avoid manually painted borders and their resize artifacts.
- Preserves the reduced history marker column, branded editor icon, and guarded hotkey-based paste behavior of 0.5.3.

The encrypted vault format and settings schema are unchanged.

**Paste behavior reminder:** open clipboard history with your global hotkey while the text cursor is in the target application. Then choose a clip and press Enter or Paste. Manually reactivating GeniaClipboard invalidates an old target by design.

## Русский

Небольшое исправление интерфейса по итогам проверки версии 0.5.3 на Windows.

- Поисковое поле снова имеет полноценную высоту.
- Текст и подсказка поиска центрируются **по вертикали** внутри поля.
- Используется штатная рамка Windows вокруг внутреннего поля ввода естественной высоты, без ручной отрисовки границ и связанных с этим артефактов.
- Сохранены компактная колонка маркеров, фирменная иконка редактора и защита автоматической вставки из 0.5.3.

Формат зашифрованного Vault и структура настроек не изменялись.

**Напоминание про вставку:** находясь в нужном приложении, откройте историю глобальной горячей клавишей; затем выберите запись и нажмите Enter или «Вставить». При ручном повторном переводе фокуса в GeniaClipboard старая цель вставки сбрасывается намеренно.
