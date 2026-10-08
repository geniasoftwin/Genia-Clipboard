# GeniaClipboard — Screenshot Gallery / Галерея скриншотов

**Status:** Screenshots have been curated from authentic Windows beta captures on 2026-10-08. **The PNG files are not yet committed to this branch.** Add them to `docs/screenshots/` before turning the image links below into embedded previews.

**Статус:** получены настоящие скриншоты beta-версии Windows, изображения отсортированы и получили понятные названия. **Сами PNG пока не загружены в эту ветку GitHub**. Перед добавлением картинок в README нужно поместить их в `docs/screenshots/`.

## Priority / Приоритет

| Order | Filename | Русский | English |
|---|---|---|---|
| 1 | `01-main-history.png` | Главное окно — история буфера | Main window and history |
| 2 | `02-main-pins-and-indicators.png` | Закреплённые и чувствительные записи | Pin and sensitive indicators |
| 3 | `03-search-filter.png` | Поиск по тексту | Search and filtering |
| 4 | `04-settings-security.png` | Настройки безопасности / Portable Vault | Security settings |
| 5 | `05-settings-history.png` | История и исключения процессов | History / exclusions |
| 6 | `06-settings-hotkeys.png` | Сочетания клавиш | Hotkeys |
| 7 | `07-settings-system.png` | Автозапуск и TXT-журнал | System and plaintext journal |
| 8 | `08-vault-modes.png` | Windows Vault / Portable Vault | Vault selection |
| 9 | `09-portable-vault-unlock.png` | Окно ввода мастер-пароля (пустое поле) | Portable Vault unlock (empty password field) |

**Review-only:** `10-edit-entry-review.png` shows a sample email and phone number. It must not be used in public-facing documentation without explicit confirmation that both values are invented demo data.

### Planned README layout / План оформления

- Intro: existing GeniaClipboard SVG brand icon, release/build/MIT badges.
- Hero: `01-main-history.png` after upload.
- Privacy: `04-settings-security.png`.
- Search & history: `03-search-filter.png` and `05-settings-history.png`.
- Additional images in a collapsible gallery, not a wall of full-size images.
- Authentic screenshots only; no simulated “features” that are not implemented.

**Do not commit:** screenshots containing actual keys, passwords, private clipboard content, personal email addresses or real telephone numbers.

## Exact Markdown to enable after PNG upload

```md
![GeniaClipboard — clipboard history](docs/screenshots/01-main-history.png)

<details>
<summary>More screenshots / Ещё скриншоты</summary>

![Security settings](docs/screenshots/04-settings-security.png)
![History and exclusions](docs/screenshots/05-settings-history.png)
![Search](docs/screenshots/03-search-filter.png)
![Hotkeys](docs/screenshots/06-settings-hotkeys.png)
![System](docs/screenshots/07-settings-system.png)

</details>
```

These links must not be enabled before the files are present.
