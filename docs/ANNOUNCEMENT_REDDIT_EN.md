# Draft for Reddit — NOT POSTED

**Suggested title:** I built an MIT-licensed, portable Windows clipboard manager with an encrypted local vault — looking for beta feedback

I’ve been working on [GeniaClipboard](https://github.com/geniasoftwin/Genia-Clipboard), a small clipboard history manager for Windows 10/11, written in C# / WinForms.

I know Ditto and CopyQ already exist (and are much more feature-rich). My goal isn't to replace them feature-for-feature. I wanted something lightweight in *workflow*, portable, and local-first, with explicit control over which text is stored.

Current features include:

- encrypted history: Windows DPAPI-backed vault or a master-password portable vault;
- a memory-only Private Session;
- process exclusions, clipboard privacy markers, and heuristic detection/expiry of some token formats;
- searching, pinning, multi-select, editing and configurable hotkeys;
- no account, cloud service or telemetry.

**Limitations:** Text-only for now; no images/files/HTML. Secret detection is heuristic, not a guarantee. TXT exports/journals are plaintext, and no clipboard manager can defend against malware with access to your desktop session. Binaries aren't Authenticode-signed yet.

We’re testing a new unified Paste workflow in v0.5.6 so that opening the UI via tray or shortcut behaves consistently. The Windows x64 binary is self-contained, and the source is under the MIT license.

If anyone wants to try it, I’d value specific feedback on the UI, focus/paste behavior, privacy defaults, and whether there is a use case where this approach feels simpler than existing tools.

GitHub: https://github.com/geniasoftwin/Genia-Clipboard

Feedback: https://github.com/geniasoftwin/Genia-Clipboard/issues

*[Attach a sanitized screenshot of the real UI once available. Read the target subreddit's current promotion/flair rules before posting; this draft is not automatically approved for any community.]*

---

Suggested destinations to verify individually: r/opensource (limited promotion; select the appropriate flair), or a permitted self-promotion thread in r/github. Do not cross-post identical messages repeatedly, and be prepared to discuss the actual technical design and limitations.
