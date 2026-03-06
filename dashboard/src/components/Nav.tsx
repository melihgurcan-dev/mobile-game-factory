"use client";
import Link from "next/link";
import { usePathname } from "next/navigation";

const links = [
  { href: "/", label: "Overview" },
  { href: "/games", label: "Games" },
  { href: "/reviews", label: "Review Inbox" },
  { href: "/agents", label: "Agents" },
];

export default function Nav() {
  const pathname = usePathname();
  return (
    <nav className="bg-gray-900 text-white px-6 py-4 flex items-center gap-8">
      <span className="font-bold text-lg tracking-tight text-indigo-400">
        🎮 Game Factory
      </span>
      <div className="flex gap-4 text-sm">
        {links.map((link) => (
          <Link
            key={link.href}
            href={link.href}
            className={`px-3 py-1 rounded transition-colors ${
              pathname === link.href
                ? "bg-indigo-600 text-white"
                : "text-gray-300 hover:text-white hover:bg-gray-700"
            }`}
          >
            {link.label}
          </Link>
        ))}
      </div>
    </nav>
  );
}
